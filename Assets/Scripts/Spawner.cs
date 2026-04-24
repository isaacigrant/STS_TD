using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
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

    private void Update()
    {
        if (!_isBetweenRounds)
        {
            Debug.Log(_currentWaveIndex);

            if (_isBetweenWaves)
            {
                _waveCooldown -= Time.deltaTime;

                if (_waveCooldown <= 0)
                {
                    _currentWaveIndex++;
                    _enemySpawnCounter = 0;
                    _enemyDestroyedCounter = 0;
                    _spawnTimer = 0;
                    _isBetweenWaves = false;

                    if (_currentWaveIndex > _currentRoundData.Waves.Length - 1)
                    {
                        _isBetweenRounds = true;
                        _isBetweenWaves = true;
                        _currentRoundIndex++;

                        if (_currentRoundIndex > _rounds.Length - 1 == false)
                        {
                            LevelUI.Instance.GetStartRoundButton().gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
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
        }
    }

    /// <summary>
    /// <para>Increases enemy destroyed counter when any enemy reaches the end.</para>
    /// </summary>
    private void HandleOnEnemyReachedEnd(EnemyData data)
    {
        _enemyDestroyedCounter++;
    }

    /// <summary>
    /// <para>Sets isBetweenRounds and isBetweenWaves to false. Resets Spawn Timer.</para>
    /// </summary>
    private void HandleOnRoundStarted()
    {
        _isBetweenRounds = false;
        _isBetweenWaves = false;
        _currentWaveIndex = 0;
        LevelUI.Instance.GetStartRoundButton().gameObject.SetActive(false);
    }

    /// <summary>
    /// <para>Gets a object from its object pool, spawns the object at this transform.position and sets the object to active.</para>
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
}
