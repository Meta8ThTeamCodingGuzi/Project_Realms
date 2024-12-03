using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlotHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
    private Slot slot;
    private bool isInventorySlot;
    private Inventory inventory;

    public bool IsInventorySlot { get => isInventorySlot; set => isInventorySlot = value; }

    public void InitializeSlotHandler(Inventory inventoryRef)
    {
        if (slot == null)
            slot = GetComponent<Slot>();

        inventory = inventoryRef;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.IsEmpty()) return;
        inventory.DragHolder.StartDrag(slot);
    }

    public void OnDrag(PointerEventData eventData)
    {
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

        // 현재 슬롯이 드래그된 아이템을 받을 수 있는지 확인
        if (slot.CanAcceptItem(dragHolder.DraggedItem))
        {
            if (slot.IsEmpty())
            {
                // 빈 슬롯인 경우 그냥 아이템 배치
                dragHolder.SetTargetSlot(slot);
            }
            else
            {
                // 아이템이 있는 경우, 두 슬롯의 아이템을 교환
                Slot sourceSlot = dragHolder.SourceSlot;

                // 원본 슬롯이 대상 아이템을 받을 수 있는지 확인
                if (sourceSlot.CanAcceptItem(slot.Item))
                {
                    // 현재 슬롯의 아이템 임시 저장
                    Item tempItem = slot.Item;

                    // 현재 슬롯을 비우고 드래그된 아이템 배치
                    slot.ClearSlot();
                    dragHolder.SetTargetSlot(slot);

                    // 원본 슬롯에 임시 저장된 아이템 배치
                    sourceSlot.PlaceItem(tempItem);
                }
            }
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInventorySlot || slot.IsEmpty()) return;

        foreach (Slot equipSlot in inventory.EquipmentSlots)
        {
            if (equipSlot.CanAcceptItem(slot.Item))
            {
                if (equipSlot.IsEmpty())
                {
                    Item itemToEquip = slot.Item;
                    slot.ClearSlot();
                    equipSlot.PlaceItem(itemToEquip);
                }
                else
                {
                    Item equippedItem = equipSlot.Item;
                    Item inventoryItem = slot.Item;

                    slot.ClearSlot();
                    equipSlot.ClearSlot();

                    equipSlot.PlaceItem(inventoryItem);
                    slot.PlaceItem(equippedItem);
                }

                if (IsInventorySlot)
                    inventory.CompactInventory();

                break;
            }
        }
    }
}