using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDragItem : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("References")]
    [SerializeField] private TowerBuildManager buildManager;

    [Header("Tower Settings")]
    [SerializeField] private int towerIndex = 0;

    private bool isDragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;

        Debug.Log("Tower drag boshlandi!");
        Debug.Log("Tower Index: " + towerIndex);

        if (buildManager == null)
        {
            Debug.LogError("TowerBuildManager ulanmagan!");
            isDragging = false;
            return;
        }

        // Tower tanlanadi
        buildManager.SelectTower(towerIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        isDragging = false;

        Debug.Log("Tower drag tugadi!");

        if (buildManager == null)
            return;

        // Tower joylashtirish
        buildManager.TryPlaceTower(
            towerIndex,
            eventData.position
        );
    }
}