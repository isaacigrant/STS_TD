using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Pathing")]
    [SerializeField] private Path _currentPath;
    [SerializeField] private int _currentWaypointIndex;

    [Header("Enemy Stats")]
    [SerializeField] private float _moveSpeed = 3f;

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
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _moveSpeed * Time.deltaTime);

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
