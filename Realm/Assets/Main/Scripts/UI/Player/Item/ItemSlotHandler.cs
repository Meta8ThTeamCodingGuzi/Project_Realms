using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private Slot slot;
    private bool isInventorySlot;
    private Inventory inventory;

    public bool IsInventorySlot { get => isInventorySlot; set => isInventorySlot = value; }

    public void InitializeSlotHandler(Inventory inventoryRef)
    {
        if (slot == null)
            slot = GetComponent<Slot>();

        inventory = inventoryRef;  // 인벤토리 참조 설정
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.IsEmpty()) return;
        inventory.DragHolder.StartDrag(slot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중에는 특별한 처리가 필요 없음
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        inventory.DragHolder.DropItem();
        if (IsInventorySlot)
            inventory.CompactInventory();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragHolder = inventory.DragHolder;
        if (!dragHolder.IsDragging) return;

        if (slot.IsEmpty() && slot.CanAcceptItem(dragHolder.DraggedItem))
            dragHolder.SetTargetSlot(slot);
    }
}