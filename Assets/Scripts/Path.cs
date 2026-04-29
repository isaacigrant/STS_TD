using UnityEngine;

/// <summary>
/// Defines a series of waypoints that form a path through the scene.
/// Attach this to an empty GameObject and assign Transform references in the Inspector.
/// </summary>
public class Path : MonoBehaviour
{
    [Header("Waypoints")]
    [Tooltip("Ordered list of Transforms that define the path.")]
    public Transform[] waypoints;

    private void OnDrawGizmosSelected()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        Gizmos.color = Color.magenta;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null)
                continue;

            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }

    /// <summary>
    /// Returns the total number of waypoints on this path.
    /// </summary>
    public int GetWaypointCount()
    {
        return waypoints.Length;
    }

    /// <summary>
    /// Returns the world-space position of the waypoint at the given index.
    /// </summary>
    /// <param name="index">Zero-based index into the waypoints array.</param>
    /// <returns>World position of the waypoint.</returns>
    public Vector3 GetWaypointPosition(int index)
    {
        return waypoints[index].position;
    }
}
