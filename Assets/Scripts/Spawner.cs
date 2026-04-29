using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the spawning of enemies across rounds and waves.
/// Reads from <see cref="RoundData"/>/<see cref="WaveData"/> ScriptableObjects and pulls enemies from <see cref="ObjectPooler"/> pools.
/// Fires <see cref="OnRoundChanged"/> whenever the round index advances so UI can react.
/// </summary>
public class Spawner : MonoBehaviour
{
    /// <summary>
    /// Fired when the round index changes. Passes the new round index as an int.
    /// </summary>
    public static event Action<int> OnRoundChanged;

    [Header("Rounds")]
    [Tooltip("Ordered array of RoundData assets that define the full level.")]
    [SerializeField] private RoundData[] _rounds;

    [Header("Pools")]
    [SerializeField] private ObjectPooler _slimePool;
    [SerializeField] private ObjectPooler _ratPool;
    [SerializeField] private ObjectPooler _oozePool;

    private Dictionary<EnemyType, ObjectPooler> _poolByEnemyType;
    private RoundData CurrentRound => _rounds[_currentRoundIndex];
    private WaveData CurrentWave => CurrentRound.Waves[_currentWaveIndex];
    private bool _isBetweenRounds = true;
    private bool _isBetweenWaves = true;
    private float _spawnTimer = 0f;
    private float _waveCooldown = 0f;
    private int _currentRoundIndex = 0;
    private int _currentWaveIndex = 0;
    private int _enemiesDestroyedThisWave = 0;
    private int _enemiesSpawnedThisWave = 0;

    private void Awake()
    {
        _poolByEnemyType = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Slime, _slimePool },
            { EnemyType.Rat, _ratPool },
            { EnemyType.Ooze, _oozePool }
        };
    }

    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        LevelUI.OnRoundStarted += HandleRoundStarted;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        LevelUI.OnRoundStarted -= HandleRoundStarted;
    }

    private void Start()
    {
        OnRoundChanged?.Invoke(_currentRoundIndex);
    }

    private void Update()
    {
        TickWaveLogic();
    }

    /// <summary>
    /// Advances to the next round and re-enables the start button.
    /// Does nothing further if all rounds are complete.
    /// </summary>
    private void AdvanceRound()
    {
        _isBetweenRounds = true;
        _isBetweenWaves = true;
        _currentRoundIndex++;

        if (_currentRoundIndex <= _rounds.Length - 1)
        {
            LevelUI.Instance.GetStartRoundButton().gameObject.SetActive(true);
            OnRoundChanged?.Invoke(_currentRoundIndex);
        }
    }

    /// <summary>
    /// Advances to the next wave, resetting per-wave counters.
    /// Calls AdvanceRound instead if this was the last wave.
    /// </summary>
    private void AdvanceWave()
    {
        _currentWaveIndex++;
        _enemiesSpawnedThisWave = 0;
        _enemiesDestroyedThisWave = 0;
        _spawnTimer = 0;
        _isBetweenWaves = false;

        if (_currentWaveIndex > CurrentRound.Waves.Length - 1)
            AdvanceRound();
    }

    /// <summary>
    /// Called when an enemy is destroyed mid-wave (e.g. killed by player).
    /// </summary>
    /// <param name="enemy">Enemy that was destroyed.</param>
    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesDestroyedThisWave++;
    }

    /// <summary>
    /// Called when an enemy reaches the end of the path without being destroyed.
    /// Still counts as removed from the wave so progression can continue.
    /// </summary>
    /// /// <param name="data">Enemy that reached the end.</param>
    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesDestroyedThisWave++;
    }

    /// <summary>
    /// Called by LevelUI when the player presses the Start Round button.
    /// Resets wave index and unpauses spawning.
    /// </summary>
    private void HandleRoundStarted()
    {
        _isBetweenRounds = false;
        _isBetweenWaves = false;
        _currentWaveIndex = 0;
        LevelUI.Instance.GetStartRoundButton().gameObject.SetActive(false);
    }

    /// <summary>
    /// Pulls an enemy of the current wave's type from the appropriate pool,
    /// positions it at the spawner's location, and activates it.
    /// <para><seealso cref="ObjectPooler.GetPooledObject"/></para>
    /// </summary>
    private void SpawnEnemy()
    {
        if (_poolByEnemyType.TryGetValue(CurrentWave.EnemyWaveType, out ObjectPooler pool))
        {
            GameObject spawnedEnemy = pool.GetPooledObject();
            spawnedEnemy.transform.position = transform.position;
            spawnedEnemy.SetActive(true);
            _enemiesSpawnedThisWave++;
        }
        else
        {
            Debug.LogWarning($"[Spawner] No pool found for EnemyType: {CurrentWave.EnemyWaveType}", this);
        }
    }

    /// <summary>
    /// Handles per-frame spawn timing during an active wave.
    /// Spawns an enemy when the timer expires, or signals wave end once
    /// all spawned enemies have been destroyed.
    /// </summary>
    private void TickSpawnLogic()
    {
        if (_enemiesSpawnedThisWave >= CurrentWave.EnemiesPerWave && _enemiesDestroyedThisWave >= CurrentWave.EnemiesPerWave)
        {
            _isBetweenWaves = true;
            _waveCooldown = CurrentWave.TimeUntilNextWave;
            return;
        }

        if (_enemiesSpawnedThisWave < CurrentWave.EnemiesPerWave)
        {
            _spawnTimer -= Time.deltaTime;

            if (_spawnTimer <= 0)
            {
                _spawnTimer = CurrentWave.SpawnInterval;
                SpawnEnemy();
            }
        }
    }

    /// <summary>
    /// Counts down the cooldown period between waves, then advances to the next wave.
    /// </summary>
    private void TickWaveCooldown()
    {
        _waveCooldown -= Time.deltaTime;

        if (_waveCooldown <= 0)
            AdvanceWave();
    }

    /// <summary>
    /// Top-level update tick. Exits early between rounds, otherwise runs the
    /// cooldown timer or the spawn logic depending on current wave state.
    /// </summary>
    private void TickWaveLogic()
    {
        if (_isBetweenRounds)
            return;

        if (_isBetweenWaves)
            TickWaveCooldown();
        else
            TickSpawnLogic();
    }
}
