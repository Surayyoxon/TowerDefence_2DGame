
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("References")]
    [SerializeField] private PathManager pathManager;
    [SerializeField] private BaseHealth baseHealth;

    private void Awake()
    {
        if (pathManager == null || baseHealth == null)
        {
            Debug.LogError("PathManager yoki BaseHealth ulanmagan!");
            enabled = false;
            return;
        }

        if (enemyPrefabs == null || enemyPrefabs.Length < 4)
        {
            Debug.LogError("4 ta Enemy Prefab kerak!");
            enabled = false;
        }
    }

    public GameObject SpawnEnemy(int enemyIndex)
    {
        if (!enabled)
            return null;

        if (enemyIndex < 0 || enemyIndex >= enemyPrefabs.Length)
        {
            Debug.LogError("Enemy index noto'g'ri: " + enemyIndex);
            return null;
        }

        GameObject prefab = enemyPrefabs[enemyIndex];

        if (prefab == null)
        {
            Debug.LogError("Enemy Prefab ulanmagan: " + enemyIndex);
            return null;
        }

        GameObject enemy = Instantiate(
            prefab,
            transform.position,
            Quaternion.identity
        );

        EnemyMovement movement =
            enemy.GetComponent<EnemyMovement>();

        if (movement == null)
        {
            Debug.LogError(prefab.name + " da EnemyMovement yo'q!");
            Destroy(enemy);
            return null;
        }

        movement.SetPath(pathManager.Waypoints);
        movement.SetBase(baseHealth);

        Debug.Log(enemy.name + " Spawn qilindi!");

        return enemy;
    }
}
