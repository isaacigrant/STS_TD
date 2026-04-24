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
    private int _currentWaveIndex = 0;
    private int _enemySpawnCounter = 0;
    private float _spawnTimer;

    private void Awake()
    {
        _enemyToPoolDictionary = new Dictionary<EnemyType, ObjectPooler>() {
            { EnemyType.Slime, _slimePool },
            { EnemyType.Rat, _ratPool },
            { EnemyType.Ooze, _oozePool }
        };
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0 && _enemySpawnCounter < _currentWaveData.EnemiesPerWave)
        {
            _spawnTimer = _currentWaveData.SpawnInterval;
            SpawnObject();
        }
        else if (_enemySpawnCounter >= _currentWaveData.EnemiesPerWave)
        {
            _currentWaveIndex = (_currentWaveIndex + 1) % _waves.Length;
            _enemySpawnCounter = 0;
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
}
