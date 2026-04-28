using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;
    
    [SerializeField] private EnemyData _enemyData;

    private Path _currentPath;
    private Vector3 _targetPosition;
    private int _currentWaypointIndex;
    private float _enemyHealth;

    public void TakeDamage(float dmg)
    {
        _enemyHealth = Mathf.Max(_enemyHealth - dmg, 0);

        if (_enemyHealth <= 0)
        {
            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        _currentPath = GameObject.Find("Path 1").GetComponent<Path>();
    }

    /// <summary>
    /// <para>Checks if Enemy is close enough to target, then if able moves to the next waypoint or else deactivates.</para>
    /// <para><see cref="Path.GetWaypointPositionFromIndex(int)"/></para>
    /// <para><see cref="OnEnemyReachedEnd"/></para>
    /// </summary>
    private void CheckDistanceToTarget()
    {
        float distanceMagnitude = (transform.position - _targetPosition).magnitude;

        if (distanceMagnitude < 0.1f)
        {
            if (_currentWaypointIndex < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypointIndex++;
                _targetPosition = _currentPath.GetWaypointPositionFromIndex(_currentWaypointIndex);
            }
            else
            {
                OnEnemyReachedEnd?.Invoke(_enemyData);
                gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        _currentWaypointIndex = 0;

        _targetPosition = _currentPath.GetWaypointPositionFromIndex(_currentWaypointIndex);

        _enemyHealth = _enemyData.EnemyHealth;
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _enemyData.EnemySpeed * Time.deltaTime);

        CheckDistanceToTarget();
    }
}

public enum EnemyType
{
    Slime,
    Rat,
    Ooze
}
