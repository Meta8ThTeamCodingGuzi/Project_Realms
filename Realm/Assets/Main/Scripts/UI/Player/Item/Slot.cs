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
            if (_item.ItemType == ItemType.Sword || _item.ItemType == ItemType.Bow)
            {
                _player.skillController.UnequipSkill(KeyCode.Mouse0);
                _player.PlayerAnimController.AnimatorChange(ItemType.None);
            }
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

                if (_item.ItemType == ItemType.Sword || _item.ItemType == ItemType.Bow)
                {
                    // ���� ���� ������Ʈ ����
                    var weaponHolder = _player.GetComponent<WeaponHolder>();
                    weaponHolder.EquipWeapon(_item.ItemData.ItemPrefab, _item.ItemType);

                    // ��ų ���
                    Skill defaultSkill = item.ItemData.GetDefaultSkillForWeapon();
                    if (defaultSkill != null)
                    {
                        _player.skillController.AddSkill(defaultSkill);
                        _player.skillController.EquipSkill(defaultSkill, KeyCode.Mouse0);

                        // ���� ������Ʈ ������Ʈ
                        if (defaultSkill is WeaponSkill weaponSkill)
                        {
                            weaponSkill.UpdateWeaponComponents();
                        }
                    }

                    _player.PlayerAnimController.AnimatorChange(_item.ItemType);
                }
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
