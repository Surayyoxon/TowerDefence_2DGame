using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject[] towerPrefabs;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private GameObject selectedTowerPrefab;
    private int selectedTowerIndex = -1;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    // Narx va pul endi bitta joyda - UIManager'da
    private bool HasEnoughMoney(int index)
    {
        return UIManager.Instance != null &&
               UIManager.Instance.GetCoins() >= UIManager.Instance.GetTowerPrice(index);
    }

    public void SelectTower(int index)
    {
        if (index < 0 || index >= towerPrefabs.Length)
        {
            Debug.LogError("Tower Index noto'g'ri: " + index);
            return;
        }

        if (towerPrefabs[index] == null)
        {
            Debug.LogError("Tower Prefab ulanmagan! Index: " + index);
            return;
        }

        if (UIManager.Instance == null)
        {
            Debug.LogError("UIManager topilmadi!");
            return;
        }

        if (!HasEnoughMoney(index))
        {
            Debug.Log("Pul yetarli emas!");
            selectedTowerPrefab = null;
            selectedTowerIndex = -1;
            return;
        }

        selectedTowerPrefab = towerPrefabs[index];
        selectedTowerIndex = index;
    }

    // TowerDragItem.OnEndDrag chaqiradi
    public bool TryPlaceTower(int index, Vector2 screenPosition)
    {
        if (selectedTowerPrefab == null || selectedTowerIndex != index)
            SelectTower(index);

        if (selectedTowerPrefab == null)
            return false;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera topilmadi!");
            return false;
        }

        if (!HasEnoughMoney(index))
        {
            Debug.Log("Pul yetarli emas!");
            return false;
        }

        Vector3 screenPos = screenPosition;
        screenPos.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        foreach (Collider2D hit in hits)
        {
            TowerPlace place = hit.GetComponentInParent<TowerPlace>();

            if (place == null)
                continue;

            if (place.TryBuildTower(selectedTowerPrefab))
            {
                // Pul UIManager orqali yechiladi -> coin UI va tower tugmalari yangilanadi
                if (!UIManager.Instance.BuyTower(index))
                    Debug.LogError("Tower qurildi, lekin pul yechilmadi!");

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