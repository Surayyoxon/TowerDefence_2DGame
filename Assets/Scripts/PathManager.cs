using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("Path Waypoints")]
    [SerializeField] private Transform[] waypoints;

    public Transform[] Waypoints => waypoints;

    public int WaypointCount => waypoints.Length;

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null)
                continue;

            Gizmos.DrawLine(
                waypoints[i].position,
                waypoints[i + 1].position
            );

            Gizmos.DrawSphere(
                waypoints[i].position,
                0.12f
            );
        }

        if (waypoints[waypoints.Length - 1] != null)
        {
            Gizmos.DrawSphere(
                waypoints[waypoints.Length - 1].position,
                0.12f
            );
        }
    }
}
