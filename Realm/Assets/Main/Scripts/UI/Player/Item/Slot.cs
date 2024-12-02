using UnityEngine;
using UnityEngine.UI;


public class Slot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private ItemType? allowedItemType = null; // null�̸� ��� Ÿ�� ���
    [SerializeField] private bool isEquipSlot = false;

    private Item _item;
    private Player _player;
    private ItemSlotHandler slotHandler;
    private StatUI statUI;

    public Item Item => _item;
    public ItemType? AllowedItemType => allowedItemType;

    public void Initialize(Player player, Inventory inventory, StatUI statUI)
    {
        _player = player;
        itemIcon.gameObject.SetActive(false);
        this.statUI = statUI;

        slotHandler = GetComponent<ItemSlotHandler>();
        if (slotHandler != null)
        {
            InitializeSlotHandler(inventory);
        }
    }

    private void InitializeSlotHandler(Inventory inventory)
    {
        // ��� �������� ���� ����
        slotHandler.IsInventorySlot = !isEquipSlot;

        // �κ��丮 ���� ���� (��� ���Կ� ���� ����)
        slotHandler.InitializeSlotHandler(inventory);
    }

    public bool IsEmpty() => _item == null;

    public bool CanAcceptItem(Item item)
    {
        if (item == null) return false;

        // ��� �����̸� ������ Ÿ�� üũ
        if (isEquipSlot && allowedItemType.HasValue)
        {
            return item.ItemType == allowedItemType.Value;
        }

        // �κ��丮 ������ ��� ������ ���
        return true;
    }

    public void PlaceItem(Item item)
    {
        if (item != null && !CanAcceptItem(item)) return;

        // ���� �������� ȿ�� ����
        if (isEquipSlot && _item != null && _player != null)
        {
            _item.RemoveStats(_player.GetComponent<ICharacterStats>());
            statUI.UpdateUI();
        }

        _item = item;

        if (_item != null)
        {
            itemIcon.sprite = _item.Icon;
            itemIcon.gameObject.SetActive(true);

            // �� ������ ȿ�� ����
            if (isEquipSlot && _player != null)
            {
                _item.ApplyStats(_player.GetComponent<ICharacterStats>());
                statUI.UpdateUI();
                // TODO : ������ ����� �÷��̾� ������ ���� ��ġ �Ʒ��� ������ ��ȯ ����
                //Instantiate(_item.ItemPrefab,_player.transform);
            }
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
        }
    }

    public void PlaceholdItem(Item item)
    {
        if (!CanAcceptItem(item)) return;

        itemIcon.sprite = item.Icon;
        itemIcon.gameObject.SetActive(true);
        itemIcon.color = new Color(1, 1, 1, 0.5f);
    }

    public void ClearSlot()
    {
        if (isEquipSlot && _item != null && _player != null)
        {
            _item.RemoveStats(_player.GetComponent<ICharacterStats>());
            statUI.UpdateUI();
        }

        _item = null;
        itemIcon.gameObject.SetActive(false);
        itemIcon.color = Color.white;
    }

    public void SetAsEquipmentSlot(ItemType type)
    {
        isEquipSlot = true;
        allowedItemType = type;

        // ���� �ڵ鷯 ���� ������Ʈ
        if (slotHandler != null)
        {
            slotHandler.IsInventorySlot = false;
        }
    }
}
