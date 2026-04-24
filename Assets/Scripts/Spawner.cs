using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Waves")]
    [SerializeField] private WaveData[] _waves;

    [Header("Pools")]
    [SerializeField] private ObjectPooler _slimePool;
    [SerializeField] private ObjectPooler _ratPool;
    [SerializeField] private ObjectPooler _oozePool;

    private Dictionary<EnemyType, ObjectPooler> _enemyToPoolDictionary;
    private WaveData _currentWaveData => _waves[_currentWaveIndex];
    private float _spawnTimer;
    private float _waveCooldown;
    private int _currentWaveIndex = 0;
    private int _enemyDestroyedCounter = 0;
    private int _enemySpawnCounter = 0;
    private bool _isBetweenRounds;
    private bool _isBetweenWaves;

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
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleOnEnemyReachedEnd;
    }

    private void Update()
    {
        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;

            if (_waveCooldown <= 0)
            {
                _currentWaveIndex = (_currentWaveIndex + 1) % _waves.Length;
                _enemySpawnCounter = 0;
                _enemyDestroyedCounter = 0;
                _spawnTimer = _currentWaveData.SpawnInterval;
                _isBetweenWaves = false;
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

    /// <summary>
    /// <para>Increases enemy destroyed counter when any enemy reaches the end.</para>
    /// </summary>
    private void HandleOnEnemyReachedEnd(EnemyData data)
    {
        _enemyDestroyedCounter++;
    }
}
