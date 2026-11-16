using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Enemy Health Bar Prefabs (har enemy uchun bittadan, UI prefab)")]
    [SerializeField] private RectTransform[] healthBarPrefabs;

    [Header("References")]
    [SerializeField] private PathManager pathManager;
    [SerializeField] private BaseHealth baseHealth;
    [SerializeField] private Canvas uiCanvas;

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
            return;
        }

        if (uiCanvas == null)
            uiCanvas = FindFirstObjectByType<Canvas>();
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

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        if (movement == null)
        {
            Debug.LogError(prefab.name + " da EnemyMovement yo'q!");
            Destroy(enemy);
            return null;
        }

        movement.SetPath(pathManager.Waypoints);
        movement.SetBase(baseHealth);

        CreateHealthBar(enemy, enemyIndex);

        return enemy;
    }

    // HP bar'ni Canvas ichida yaratib, EnemyHealth'ga ulaydi
    private void CreateHealthBar(GameObject enemy, int enemyIndex)
    {
        // EnemyHealth root'da bo'lmasa, bola obyektlardan ham qidiradi
        EnemyHealth health = enemy.GetComponentInChildren<EnemyHealth>(true);

        if (health == null)
        {
            Debug.LogError(enemy.name + " da EnemyHealth skripti topilmadi, HP bar yaratilmadi!");
            return;
        }

        if (healthBarPrefabs == null ||
            enemyIndex >= healthBarPrefabs.Length ||
            healthBarPrefabs[enemyIndex] == null)
        {
            Debug.LogWarning("Health Bar Prefab ulanmagan: " + enemyIndex);
            return;
        }

        if (uiCanvas == null)
        {
            Debug.LogWarning("Canvas topilmadi, HP bar yaratilmadi!");
            return;
        }

        // UIManager yaratgan konteyner (Game Panel ichida), bo'lmasa Canvas
        Transform parent =
            (UIManager.Instance != null && UIManager.Instance.EnemyBarContainer != null)
            ? UIManager.Instance.EnemyBarContainer
            : uiCanvas.transform;

        RectTransform bar = Instantiate(healthBarPrefabs[enemyIndex], parent);

        // EnemyHealth anchoredPosition'ni Canvas markaziga nisbatan hisoblaydi,
        // shuning uchun anchor markazda bo'lishi shart
        bar.anchorMin = new Vector2(0.5f, 0.5f);
        bar.anchorMax = new Vector2(0.5f, 0.5f);

        // Ramka (frame) va yashil line (fill) alohida Image'lar: fill avtomatik topiladi
        Image fill = FindFillImage(bar);

        if (fill == null)
        {
            Debug.LogError(
                "Health Bar prefabida yashil line (Fill) topilmadi! " +
                "Prefab ichida ramka va line ikkita alohida Image bo'lishi kerak."
            );
            Destroy(bar.gameObject);
            return;
        }

        health.SetHealthBar(bar, fill);

        Debug.Log("HP bar yaratildi: " + enemy.name + " | parent: " + parent.name);
    }

    // =====================================
    // FILL TOPISH
    // =====================================
    private bool IsFrameName(string imageName)
    {
        string n = imageName.ToLowerInvariant();

        return n.Contains("frame") || n.Contains("outline") ||
               n.Contains("border") || n.Contains("back") ||
               n.Contains("bg");
    }

    private Image FindFillImage(RectTransform bar)
    {
        Image[] images = bar.GetComponentsInChildren<Image>(true);

        Image fill = null;

        // 1) Image Type = Filled bo'lsa
        foreach (Image img in images)
        {
            if (img.type == Image.Type.Filled)
            {
                fill = img;
                break;
            }
        }

        // 2) Nomi 'fill' yoki 'line' bo'lsa (ramka nomlari hisobga olinmaydi)
        if (fill == null)
        {
            foreach (Image img in images)
            {
                string n = img.name.ToLowerInvariant();

                if (IsFrameName(img.name))
                    continue;

                if (n.Contains("fill") || n.Contains("line"))
                {
                    fill = img;
                    break;
                }
            }
        }

        // 3) Hech biri mos kelmasa: eng ustidagi (oxirgi) Image - line,
        //    ramka esa uning orqasida (birinchi) turadi
        if (fill == null && images.Length >= 2)
            fill = images[images.Length - 1];

        if (fill == null)
            return null;

        // Line chapdan o'ngga kamayadi
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 1f;

        // HP bar sichqoncha bosishlarini to'smasligi uchun
        foreach (Image img in images)
            img.raycastTarget = false;

        return fill;
    }
}