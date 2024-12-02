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

        inventory = inventoryRef;  // �κ��丮 ���� ����
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot.IsEmpty()) return;
        inventory.DragHolder.StartDrag(slot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �巡�� �߿��� Ư���� ó���� �ʿ� ����
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

        // ���� ������ �巡�׵� �������� ���� �� �ִ��� Ȯ��
        if (slot.CanAcceptItem(dragHolder.DraggedItem))
        {
            if (slot.IsEmpty())
            {
                // �� ������ ��� �׳� ������ ��ġ
                dragHolder.SetTargetSlot(slot);
            }
            else
            {
                // �������� �ִ� ���, �� ������ �������� ��ü
                Slot sourceSlot = dragHolder.SourceSlot;

                // ���� ������ �������� ���� ���Կ� �� �� �ִ��� Ȯ��
                if (sourceSlot.CanAcceptItem(slot.Item))
                {
                    // ������ ��ü
                    Item tempItem = slot.Item;

                    // ���� ������ �������� ���� �巡�׵� ������ ��ġ
                    slot.ClearSlot();
                    dragHolder.SetTargetSlot(slot);

                    // ���� ���Կ� ���� ������ ��ġ
                    sourceSlot.PlaceItem(tempItem);
                }
            }
        }

        if (IsInventorySlot)
            inventory.CompactInventory();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // �κ��丮 ������ �ƴϰų� �� �����̸� ����
        if (!IsInventorySlot || slot.IsEmpty()) return;

        // ��� ���Ե��� ã�Ƽ� ������ ���Կ� ������ ���� �õ�
        foreach (Slot equipSlot in inventory.EquipmentSlots)
        {
            // �ش� ������ Ÿ���� ���� �� �ִ� �������� Ȯ��
            if (equipSlot.CanAcceptItem(slot.Item))
            {
                if (equipSlot.IsEmpty())
                {
                    // �� ��� ���Կ� ����
                    Item itemToEquip = slot.Item;
                    slot.ClearSlot();
                    equipSlot.PlaceItem(itemToEquip);
                }
                else
                {
                    // �̹� ������ �����۰� ��ü
                    Item equippedItem = equipSlot.Item;
                    Item inventoryItem = slot.Item;

                    slot.ClearSlot();
                    equipSlot.ClearSlot();

                    equipSlot.PlaceItem(inventoryItem);
                    slot.PlaceItem(equippedItem);
                }

                // �κ��丮 ����
                if (IsInventorySlot)
                    inventory.CompactInventory();

                break;
            }
        }
    }
}