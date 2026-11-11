
using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject[] towerPrefabs;

    [Header("Tower Prices")]
    [SerializeField] private int[] towerPrices = { 100, 150, 200, 300 };

    [Header("Economy")]
    [SerializeField] private EconomyManager economyManager;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private GameObject selectedTowerPrefab;
    private int selectedTowerIndex = -1;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void SelectTower(int index)
    {
        if (index < 0 || index >= towerPrefabs.Length)
        {
            Debug.LogError("Tower Index noto'g'ri: " + index);
            return;
        }

        if (index >= towerPrices.Length)
        {
            Debug.LogError("Tower narxi belgilanmagan!");
            return;
        }

        if (towerPrefabs[index] == null)
        {
            Debug.LogError("Tower Prefab ulanmagan! Index: " + index);
            return;
        }

        if (economyManager == null)
        {
            Debug.LogError("EconomyManager ulanmagan!");
            return;
        }

        // Pulni tekshirish
        int price = towerPrices[index];

        if (!economyManager.HasEnoughMoney(price))
        {
            Debug.Log("Pul yetarli emas! Kerakli pul: " + price);
            selectedTowerPrefab = null;
            selectedTowerIndex = -1;
            return;
        }

        selectedTowerPrefab = towerPrefabs[index];
        selectedTowerIndex = index;

        Debug.Log(
            "Tanlandi: " + selectedTowerPrefab.name +
            " | Price: " + price
        );
    }

    // TowerDragItem.OnEndDrag chaqiradi
    public bool TryPlaceTower(int index, Vector2 screenPosition)
    {
        // Tanlangan tower mavjudligini tekshirish
        if (selectedTowerPrefab == null || selectedTowerIndex != index)
        {
            SelectTower(index);
        }

        if (selectedTowerPrefab == null)
            return false;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera topilmadi!");
            return false;
        }

        if (economyManager == null)
        {
            Debug.LogError("EconomyManager ulanmagan!");
            return false;
        }

        int towerPrice = towerPrices[index];

        // Pulni yana bir marta tekshirish
        if (!economyManager.HasEnoughMoney(towerPrice))
        {
            Debug.Log("Pul yetarli emas!");
            return false;
        }

        // Screen Position -> World Position
        Vector3 screenPos = screenPosition;

        screenPos.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector2 worldPos =
            mainCamera.ScreenToWorldPoint(screenPos);

        // TowerPlace aniqlash
        Collider2D[] hits =
            Physics2D.OverlapPointAll(worldPos);

        Debug.Log("Topilgan collider soni: " + hits.Length);

        foreach (Collider2D hit in hits)
        {
            TowerPlace place =
                hit.GetComponentInParent<TowerPlace>();

            if (place == null)
                continue;

            // Tower qurish
            if (place.TryBuildTower(selectedTowerPrefab))
            {
                // Faqat muvaffaqiyatli qurilgandan keyin pul ayiriladi
                bool spent = economyManager.TrySpendMoney(towerPrice);

                if (!spent)
                {
                    Debug.LogError("Tower qurildi, lekin pul yechilmadi!");
                    return false;
                }

                Debug.Log(
                    selectedTowerPrefab.name +
                    " sotib olindi! -" + towerPrice
                );

                selectedTowerPrefab = null;
                selectedTowerIndex = -1;

                return true;
            }

            Debug.Log("Bu TowerPlace band!");
            return false;
        }

        Debug.LogWarning("TowerPlace topilmadi!");
        selectedTowerPrefab = null;
        selectedTowerIndex = -1;

        return false;
    }
}
