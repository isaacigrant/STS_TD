using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerData _towerData;

    private List<Enemy> _enemiesInRange;
    private BoxCollider2D _towerCollider;
    private ObjectPooler _projectilePool;
    private float _shotTimer;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(_towerData.TowerRange, _towerData.TowerRange));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            _enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            
            if (_enemiesInRange.Contains(enemy))
            {
                _enemiesInRange.Remove(enemy);
            }
        }
    }

    private void Start()
    {
        _projectilePool = GetComponent<ObjectPooler>();
        _towerCollider = GetComponent<BoxCollider2D>();

        _towerCollider.size = new Vector2(_towerData.TowerRange, _towerData.TowerRange);
        _enemiesInRange = new List<Enemy>();
        _shotTimer = _towerData.TowerAttackSpeed;
    }

    private void ShootTowerProjectile()
    {
        if (_enemiesInRange.Count > 0)
        {
            GameObject proj = _projectilePool.GetObjectInPool();
            proj.transform.position = transform.position;
            proj.SetActive(true);
            Vector2 projectileDirection = (_enemiesInRange[0].transform.position - transform.position).normalized;
            proj.GetComponent<Projectile>().ShootProjectile(_towerData, projectileDirection);
        }
    }

    private void Update()
    {
        _shotTimer -= Time.deltaTime;

        if (_shotTimer <= 0)
        {
            _shotTimer = _towerData.TowerAttackSpeed;
            ShootTowerProjectile();
        }
    }
}
