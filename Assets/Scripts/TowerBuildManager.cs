using UnityEngine;

public class TowerBuildManager : MonoBehaviour
{
    [Header("Tower Prefabs")]
    [SerializeField] private GameObject[] towerPrefabs;

    [Header("Camera")]
    [SerializeField] private Camera mainCamera;

    private GameObject selectedTowerPrefab;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void SelectTower(int index)
    {
        if (index < 0 || index >= towerPrefabs.Length)
        {
            Debug.LogError("Index noto'g'ri: " + index);
            return;
        }

        if (towerPrefabs[index] == null)
        {
            Debug.LogError("Tower Prefab ulanmagan! Index: " + index);
            return;
        }

        selectedTowerPrefab = towerPrefabs[index];
        Debug.Log("Tanlandi: " + selectedTowerPrefab.name);
    }

    // TowerDragItem.OnEndDrag shuni chaqiradi
    public bool TryPlaceTower(int index, Vector2 screenPosition)
    {
        // Drag boshlanganda tanlanmagan bo'lsa, shu yerda tanlaymiz
        if (selectedTowerPrefab == null)
            SelectTower(index);

        if (selectedTowerPrefab == null)
            return false;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera topilmadi!");
            return false;
        }

        Vector3 screenPos = screenPosition;
        screenPos.z = Mathf.Abs(mainCamera.transform.position.z);
        Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPos);

        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);
        Debug.Log("Topilgan collider soni: " + hits.Length);

        foreach (Collider2D hit in hits)
        {
            TowerPlace place = hit.GetComponentInParent<TowerPlace>();
            if (place == null)
                continue;

            if (place.TryBuildTower(selectedTowerPrefab))
            {
                selectedTowerPrefab = null;
                return true;
            }

            return false; // joy band
        }

        Debug.LogWarning("TowerPlace topilmadi! Collider2D bormi?");
        selectedTowerPrefab = null; // noto'g'ri joyga tashlansa tanlov bekor bo'ladi
        return false;
    }
}