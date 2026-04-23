using UnityEngine;

public class Path : MonoBehaviour
{
    public Transform[] Waypoints;

    private void OnDrawGizmos()
    {
        if (Waypoints.Length > 0)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                Gizmos.color = Color.purple;
                
                if (i < Waypoints.Length - 1)
                {
                    Gizmos.DrawLine(Waypoints[i].position, Waypoints[i + 1].position);
                }
            }
        }
    }
}
