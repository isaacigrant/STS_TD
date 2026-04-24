using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Pathing")]
    [SerializeField] private Path m_CurrentPath;
    [SerializeField] private int m_CurrentWaypointIndex;

    [Header("Enemy Stats")]
    [SerializeField] private float m_MoveSpeed = 3f;

    private Vector3 m_TargetPosition;

    private void Awake()
    {
        m_CurrentPath = GameObject.Find("Path 1").GetComponent<Path>();
    }

    private void OnEnable()
    {
        m_CurrentWaypointIndex = 0;
        m_TargetPosition = m_CurrentPath.GetPositionFromIndex(m_CurrentWaypointIndex);
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_TargetPosition, m_MoveSpeed * Time.deltaTime);

        float distanceMagnitude = (transform.position - m_TargetPosition).magnitude;

        if (distanceMagnitude < 0.1f)
        {
            if (m_CurrentWaypointIndex < m_CurrentPath.Waypoints.Length - 1)
            {
                m_CurrentWaypointIndex++;
                m_TargetPosition = m_CurrentPath.GetPositionFromIndex(m_CurrentWaypointIndex);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
