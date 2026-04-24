using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private ObjectPooler _objectPool;
    [SerializeField] private float _spawnEnemiesInterval;

    private float _spawnTimer;

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0)
        {
            _spawnTimer = _spawnEnemiesInterval;
            SpawnEnemy();
        }
    }

    /// <summary>
    /// <para>Gets a object from its object pool, spawns the object at this transform.position and sets the object to active.</para>
    /// <see cref="ObjectPooler.GetObjectInPool"/>
    /// </summary>
    private void SpawnEnemy()
    {
        GameObject spawnedEnemy = _objectPool.GetObjectInPool();
        spawnedEnemy.transform.position = transform.position;
        spawnedEnemy.SetActive(true);
    }
}
