using UnityEngine;
using System;

/// <summary>
/// Controls an enemy's movement along a <see cref="Path"/>, handles damage, and fires
/// events when the enemy either reaches the end or is destroyed.
/// Retrieved from an <see cref="ObjectPooler"/> — OnEnable resets state for reuse.
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>Fired when this enemy reaches the final waypoint. Passes its <seealso cref="EnemyData"/>.</summary>
    public static event Action<EnemyData> OnEnemyReachedEnd;
    /// <summary>Fired when this enemy's health reaches zero. Passes itself for identification.</summary>
    public static event Action<Enemy> OnEnemyDestroyed;

    [Tooltip("ScriptableObject defining this enemy's stats (speed, health, etc.)")]
    [SerializeField] private EnemyData _enemyData;

    private Path _path;
    private Vector3 _targetPosition;
    private const float _WAYPOINT_MINIMUM_DISTANCE = 0.1f;
    private float _currentHealth;
    private int _waypointIndex;

    private void Awake()
    {
        _path = GameObject.Find("Path 1").GetComponent<Path>();
    }

    private void OnEnable()
    {
        _waypointIndex = 0;

        _targetPosition = _path.GetWaypointPosition(_waypointIndex);

        _currentHealth = _enemyData.EnemyHealth;
    }

    private void Update()
    {
        MoveTowardsTarget();
        CheckWaypointDistance();
    }

    /// <summary>
    /// Checks whether the enemy has arrived at its current waypoint.
    /// Advances to the next waypoint, or fires <see cref="OnEnemyReachedEnd"/> and
    /// deactivates the enemy if the final waypoint has been reached.
    /// </summary>
    private void CheckWaypointDistance()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _targetPosition);

        if (distanceToTarget > _WAYPOINT_MINIMUM_DISTANCE)
            return;

        if (_waypointIndex >= _path.GetWaypointCount() - 1)
        {
            OnEnemyReachedEnd?.Invoke(_enemyData);
            gameObject.SetActive(false);
        }
        else
        {
            _waypointIndex++;
            _targetPosition = _path.GetWaypointPosition(_waypointIndex);
        }
    }

    /// <summary>
    /// Steps the enemy toward its current target waypoint this frame.
    /// </summary>
    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _enemyData.EnemySpeed * Time.deltaTime);
    }

    /// <summary>
    /// Applies damage to this enemy. Fires <see cref="OnEnemyDestroyed"/> and deactivates
    /// the enemy if health reaches zero.
    /// </summary>
    /// <param name="damage">Amount of damage to deal. Clamped so health never goes below zero.</param>
    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);

        if (_currentHealth <= 0)
        {
            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}

/// <summary>
/// Identifies which enemy type a wave or pool entry refers to.
/// Must match the keys registered in Spawner's _poolByEnemyType dictionary.
/// </summary>
public enum EnemyType
{
    Slime,
    Rat,
    Ooze
}
