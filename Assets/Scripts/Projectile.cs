using UnityEngine;

/// <summary>
/// Controls a pooled projectile's movement, lifetime, and hit detection.
/// Initialised by <see cref="Tower"/> via <see cref="ShootProjectile(TowerData, Vector3)"/> each time it is pulled from the pool.
/// </summary>
public class Projectile : MonoBehaviour
{
    private Vector2 _direction;
    private TowerData _towerData;
    private const float _DEFAULT_LIFETIME = 3f;
    private float _lifetime;

    private void Update()
    {
        TickLifetime();
        MoveProjectile();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;

        Enemy enemy = collision.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(_towerData.TowerDamage);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Moves the projectile along its direction vector at the tower's projectile speed.
    /// </summary>
    private void MoveProjectile()
    {
        transform.position += (Vector3)_direction * _towerData.TowerProjectileSpeed * Time.deltaTime;
    }

    /// <summary>
    /// Counts down the projectile's remaining lifetime and deactivates
    /// it when the timer expires so it returns to the pool.
    /// </summary>
    private void TickLifetime()
    {
        _lifetime -= Time.deltaTime;

        if (_lifetime <= 0)
            gameObject.SetActive(false);
    }

    /// <summary>
    /// Initialises the projectile for a new shot. Call this immediately after
    /// retrieving the object from the pool and before SetActive(true).
    /// </summary>
    /// <param name="towerData">Stats used for speed and damage.</param>
    /// <param name="direction">Normalised direction vector toward the target.</param>
    public void ShootProjectile(TowerData towerData, Vector3 direction)
    {
        _towerData = towerData;
        _direction = direction;
        _lifetime = _DEFAULT_LIFETIME;
    }
}
