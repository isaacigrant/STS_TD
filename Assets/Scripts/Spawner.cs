using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static event Action<int> OnRoundChanged;

    [Header("Rounds")]
    [SerializeField] private RoundData[] _rounds;

    [Header("Pools")]
    [SerializeField] private ObjectPooler _slimePool;
    [SerializeField] private ObjectPooler _ratPool;
    [SerializeField] private ObjectPooler _oozePool;

    private Dictionary<EnemyType, ObjectPooler> _enemyToPoolDictionary;
    private RoundData _currentRoundData => _rounds[_currentRoundIndex];
    private WaveData _currentWaveData => _currentRoundData.Waves[_currentWaveIndex];
    private float _spawnTimer = 0;
    private float _waveCooldown;
    private int _currentRoundIndex = 0;
    private int _currentWaveIndex = 0;
    private int _enemyDestroyedCounter = 0;
    private int _enemySpawnCounter = 0;
    private bool _isBetweenRounds = true;
    private bool _isBetweenWaves = true;

    private void Awake()
    {
        _enemyToPoolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Slime, _slimePool },
            { EnemyType.Rat, _ratPool },
            { EnemyType.Ooze, _oozePool }
        };
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleOnEnemyReachedEnd;
        LevelUI.OnRoundStarted += HandleOnRoundStarted;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleOnEnemyReachedEnd;
        LevelUI.OnRoundStarted -= HandleOnRoundStarted;
    }

    private void Start()
    {
        OnRoundChanged?.Invoke(_currentRoundIndex);
    }

    private void Update()
    {
        WaveLogic();
    }

    /// <summary>
    /// <para>Sets the state to transition between rounds and waves, increments the round index, 
    /// and enables the start round button if there are remaining rounds in the level.</para>
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
    /// <para>Increments the wave index, resets spawn and destruction counters, and checks current wave index to <see cref="AdvanceRound"/> or if more waves remain in the current round.</para>
    /// </summary>
    private void AdvanceWave()
    {
        _currentWaveIndex++;
        _enemySpawnCounter = 0;
        _enemyDestroyedCounter = 0;
        _spawnTimer = 0;
        _isBetweenWaves = false;

        if (_currentWaveIndex > _currentRoundData.Waves.Length - 1)
            AdvanceRound();
    }

    /// <summary>
    /// <para>Increases enemy destroyed counter when any enemy reaches the end.</para>
    /// </summary>
    private void HandleOnEnemyReachedEnd(EnemyData data)
    {
        _enemyDestroyedCounter++;
    }

    /// <summary>
    /// <para>Sets isBetweenRounds and isBetweenWaves to false. Resets current wave index and turns off Next Round button.</para>
    /// </summary>
    private void HandleOnRoundStarted()
    {
        _isBetweenRounds = false;
        _isBetweenWaves = false;
        _currentWaveIndex = 0;
        LevelUI.Instance.GetStartRoundButton().gameObject.SetActive(false);
    }

    /// <summary>
    /// <para>Manages enemy spawning based on timing and wave population. Triggers object spawning at defined intervals 
    /// or sets the transition state and cooldown once all enemies in the current wave are destroyed.</para>
    /// <see cref="SpawnObject"/>
    /// </summary>
    private void SpawnLogic()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0 && _enemySpawnCounter < _currentWaveData.EnemiesPerWave)
        {
            _spawnTimer = _currentWaveData.SpawnInterval;
            SpawnObject();
        }
        else if (_enemySpawnCounter >= _currentWaveData.EnemiesPerWave && _enemyDestroyedCounter >= _currentWaveData.EnemiesPerWave)
        {
            _isBetweenWaves = true;
            _waveCooldown = _currentWaveData.TimeUntilNextWave;
        }
    }

    /// <summary>
    /// <para>Attempts to retrieve an object pool based on the current wave's enemy type. If successful, 
    /// retrieves an object from the pool, positions it at the spawner, activates it, and increments the spawn counter.</para>
    /// <see cref="ObjectPooler.GetObjectInPool"/>
    /// </summary>
    private void SpawnObject()
    {
        if (_enemyToPoolDictionary.TryGetValue(_currentWaveData.EnemyWaveType, out var pool))
        {
            GameObject spawnedObject = pool.GetObjectInPool();
            spawnedObject.transform.position = transform.position;
            spawnedObject.SetActive(true);
            _enemySpawnCounter++;
        }
    }

    /// <summary>
    /// <para>Determines the current wave state, exits early if between rounds, otherwise processes either 
    /// the <see cref="WaveCooldown"/> or the active <see cref="SpawnLogic"/> based on the current transition status.</para>
    /// </summary>
    private void WaveLogic()
    {
        if (_isBetweenRounds)
            return;

        if (_isBetweenWaves)
            WaveCooldown();
        else
            SpawnLogic();
    }

    /// <summary>
    /// <para>Decrements the wave cooldown timer by the elapsed frame time and calls <see cref="AdvanceWave"/> 
    /// once the timer reaches zero.</para>
    /// </summary>
    private void WaveCooldown()
    {
        _waveCooldown -= Time.deltaTime;

        if (_waveCooldown > 0)
            return;

        AdvanceWave();
    }
}
