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
                // 아이템이 있는 경우, 두 슬롯의 아이템을 교체
                Slot sourceSlot = dragHolder.SourceSlot;

                // 현재 슬롯의 아이템이 원본 슬롯에 들어갈 수 있는지 확인
                if (sourceSlot.CanAcceptItem(slot.Item))
                {
                    // 아이템 교체
                    Item tempItem = slot.Item;

                    // 현재 슬롯의 아이템을 비우고 드래그된 아이템 배치
                    slot.ClearSlot();
                    dragHolder.SetTargetSlot(slot);

                    // 원본 슬롯에 이전 아이템 배치
                    sourceSlot.PlaceItem(tempItem);
                }
            }
        }

        if (IsInventorySlot)
            inventory.CompactInventory();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 인벤토리 슬롯이 아니거나 빈 슬롯이면 무시
        if (!IsInventorySlot || slot.IsEmpty()) return;

        // 장비 슬롯들을 찾아서 적절한 슬롯에 아이템 장착 시도
        foreach (Slot equipSlot in inventory.EquipmentSlots)
        {
            // 해당 아이템 타입을 받을 수 있는 슬롯인지 확인
            if (equipSlot.CanAcceptItem(slot.Item))
            {
                if (equipSlot.IsEmpty())
                {
                    // 빈 장비 슬롯에 장착
                    Item itemToEquip = slot.Item;
                    slot.ClearSlot();
                    equipSlot.PlaceItem(itemToEquip);
                }
                else
                {
                    // 이미 장착된 아이템과 교체
                    Item equippedItem = equipSlot.Item;
                    Item inventoryItem = slot.Item;

                    slot.ClearSlot();
                    equipSlot.ClearSlot();

                    equipSlot.PlaceItem(inventoryItem);
                    slot.PlaceItem(equippedItem);
                }

                // 인벤토리 정리
                if (IsInventorySlot)
                    inventory.CompactInventory();

                break;
            }
        }
    }
}