using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Pathing")]
    [SerializeField] private int _currentWaypointIndex;
    private Path _currentPath;

    [Header("Enemy Data")]
    [SerializeField] private EnemyData _enemyData;

    private Vector3 _targetPosition;

    private void Awake()
    {
        _currentPath = GameObject.Find("Path 1").GetComponent<Path>();
    }

    private void OnEnable()
    {
        _currentWaypointIndex = 0;

        _targetPosition = _currentPath.GetWaypointPositionFromIndex(_currentWaypointIndex);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _enemyData.EnemySpeed * Time.deltaTime);

        CheckDistanceToTarget();
    }

    /// <summary>
    /// <para>Checks if Enemy is close enough to target, then if able moves to the next waypoint or else deactivates.</para>
    /// <see cref="Path.GetWaypointPositionFromIndex(int)"/>
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
                gameObject.SetActive(false);
            }
        }
    }
}

public enum EnemyType
{
    Slime,
    Rat,
    Ooze
}
