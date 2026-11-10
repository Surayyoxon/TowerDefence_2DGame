using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyStats stats;
    private Transform[] waypoints;

    private BaseHealth baseHealth;

    private int currentWaypointIndex = 0;

    private bool pathAssigned = false;
    private bool reachedBase = false;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();

        if (stats == null)
        {
            Debug.LogError(gameObject.name + " da EnemyStats yo'q!");
            enabled = false;
        }
    }

    public void SetPath(Transform[] path)
    {
        if (path == null || path.Length == 0)
        {
            Debug.LogError("Enemy uchun Path topilmadi!");
            return;
        }

        waypoints = path;

        transform.position = waypoints[0].position;

        currentWaypointIndex = 1;
        pathAssigned = true;
    }

    public void SetBase(BaseHealth targetBase)
    {
        baseHealth = targetBase;
    }

    private void Update()
    {
        if (!pathAssigned || reachedBase)
            return;

        if (currentWaypointIndex >= waypoints.Length)
        {
            ReachBase();
            return;
        }

        MoveToWaypoint();
    }

    private void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];

        if (target == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            stats.MoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentWaypointIndex++;
        }
    }

    private void ReachBase()
    {
        reachedBase = true;

        if (baseHealth != null)
        {
            baseHealth.TakeDamage(stats.BaseDamage);
        }
        else
        {
            Debug.LogError("Enemy uchun BaseHealth ulanmagan!");
        }

        Debug.Log(gameObject.name + " Base ga yetib keldi!");

        Destroy(gameObject);
    }
}
