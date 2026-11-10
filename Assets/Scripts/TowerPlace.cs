using UnityEngine;

public class TowerPlace : MonoBehaviour
{
    [Header("Auto Fit")]
    [Tooltip("Minorani TowerPlace o'lchamiga avtomatik moslash")]
    [SerializeField] private bool autoFit = true;

    [Tooltip("1 = poydevor joy kollideri kengligiga teng. Kattaroq qiymat = minora kattaroq")]
    [SerializeField] private float fitMultiplier = 1.1f;

    [Tooltip("Minora rasmlari TowerPlace rasmidan oldinroq (ustida) chizilishi uchun")]
    [SerializeField] private int sortingOrderOffset = 10;

    private bool isOccupied = false;

    public bool IsOccupied => isOccupied;

    public bool TryBuildTower(GameObject towerPrefab)
    {
        if (isOccupied)
        {
            Debug.Log("Bu joy band!");
            return false;
        }

        if (towerPrefab == null)
        {
            Debug.LogError("Tower Prefab null!");
            return false;
        }

        // Prefab pivoti poydevor markazida, shuning uchun aynan joy markaziga qo'yamiz
        GameObject tower = Instantiate(
            towerPrefab,
            transform.position,
            Quaternion.identity
        );

        if (autoFit)
            FitToPlace(tower);

        // Joy rasmi ustida ko'rinishi uchun
        int baseOrder = 0;
        SpriteRenderer placeSprite = GetComponent<SpriteRenderer>();
        if (placeSprite != null)
            baseOrder = placeSprite.sortingOrder;

        foreach (SpriteRenderer sr in tower.GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sortingLayerID = placeSprite != null ? placeSprite.sortingLayerID : sr.sortingLayerID;
            sr.sortingOrder += baseOrder + sortingOrderOffset;
        }

        tower.transform.SetParent(transform, true);

        isOccupied = true;

        Debug.Log(
            "Tower qurildi: " + tower.name +
            " | Position: " + transform.position
        );

        return true;
    }

    private void FitToPlace(GameObject tower)
    {
        // Joy kengligi: kollider (aylana) bo'yicha, bo'lmasa sprite bo'yicha
        float placeWidth = 0f;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            placeWidth = col.bounds.size.x;
        else
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                placeWidth = sr.bounds.size.x;
        }

        if (placeWidth <= 0f)
            return;

        // Minora kengligi: eng keng rasm (poydevor) bo'yicha
        float towerWidth = 0f;
        foreach (SpriteRenderer sr in tower.GetComponentsInChildren<SpriteRenderer>())
            towerWidth = Mathf.Max(towerWidth, sr.bounds.size.x);

        if (towerWidth <= 0f)
            return;

        float scale = (placeWidth / towerWidth) * fitMultiplier;

        // Ildiz poydevor markazida turibdi, shuning uchun markaz joyida qoladi
        tower.transform.localScale = tower.transform.localScale * scale;
    }
}