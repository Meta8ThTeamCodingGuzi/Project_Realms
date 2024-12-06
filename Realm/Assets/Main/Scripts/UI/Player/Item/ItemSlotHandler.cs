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

        if (slot.CanAcceptItem(dragHolder.DraggedItem))
        {
            if (slot.IsEmpty())
            {
                dragHolder.SetTargetSlot(slot);
            }
            else
            {
                Slot sourceSlot = dragHolder.SourceSlot;

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
        if (slot.IsEmpty()) return;

        if (IsInventorySlot)
        {
            // 인벤토리 슬롯 클릭 시 - 장비 장착 로직
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

                    inventory.CompactInventory();
                    break;
                }
            }
        }
        else
        {
            // 장비 슬롯 클릭 시 - 장비 해제 로직
            if (inventory.HasFreeInventorySpace())
            {
                Item unequippedItem = slot.Item;  // 먼저 아이템 참조를 저장
                slot.ClearSlot();                 // 슬롯 비우기
                inventory.TryAddItem(unequippedItem);  // 저장해둔 아이템 참조 사용
                inventory.CompactInventory();
            }
        }
    }
}