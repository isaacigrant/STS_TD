using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a tower's targeting and shooting behaviour.
/// Tracks enemies entering/exiting its collider range, and fires a pooled
/// projectile at the first <see cref="Enemy"/> in range on a repeating attack-speed timer.
/// Requires a BoxCollider2D (set to Trigger) and an <see cref="ObjectPooler"/> on the same GameObject.
/// </summary>
public class Tower : MonoBehaviour
{
    [Tooltip("ScriptableObject defining this tower's stats (range, attack speed, damage, etc.)")]
    [SerializeField] private TowerData _towerData;

    private List<Enemy> _enemiesInRange;
    private BoxCollider2D _rangeCollider;
    private ObjectPooler _projectilePool;
    private float _attackTimer;

    private void Start()
    {
        _projectilePool = GetComponent<ObjectPooler>();
        _rangeCollider = GetComponent<BoxCollider2D>();

        _enemiesInRange = new List<Enemy>();
        _rangeCollider.size = new Vector2(_towerData.TowerRange, _towerData.TowerRange);
        _attackTimer = 0;
    }

    private void Update()
    {
        TickAttackTimer();
    }

    private void OnDrawGizmosSelected()
    {
        if (_towerData == null)
            return;

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, new Vector3(_towerData.TowerRange, _towerData.TowerRange, 0f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
            _enemiesInRange.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        Enemy enemy = collision.gameObject.GetComponent<Enemy>();

        if (enemy != null)
            _enemiesInRange.Remove(enemy);
    }

    /// <summary>
    /// Pulls a <see cref="Projectile"/> from the <see cref="ObjectPooler"/>, positions it at the tower,
    /// and launches it toward the first <see cref="Enemy"/> in the range list.
    /// </summary>
    private void FireAtFirstEnemy()
    {
        Enemy target = _enemiesInRange[0];

        GameObject projectileObj = _projectilePool.GetPooledObject();
        projectileObj.transform.position = transform.position;
        projectileObj.SetActive(true);

        Vector2 directionToTarget = (target.transform.position - transform.position).normalized;
        projectileObj.GetComponent<Projectile>().ShootProjectile(_towerData, directionToTarget);
    }

    /// <summary>
    /// Counts down the attack timer and fires when it expires,
    /// provided at least one <see cref="Enemy"/> is in range.
    /// </summary>
    private void TickAttackTimer()
    {
        if (_enemiesInRange.Count == 0)
            return;

        _attackTimer -= Time.deltaTime;

        if (_attackTimer <= 0)
        {
            _attackTimer = _towerData.TowerAttackSpeed;
            FireAtFirstEnemy();
        }
    }
}
