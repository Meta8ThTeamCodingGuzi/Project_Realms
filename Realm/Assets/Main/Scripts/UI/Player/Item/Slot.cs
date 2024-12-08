using UnityEngine;
using UnityEngine.UI;


public class Slot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private ItemType? allowedItemType = null; // null̸  Ÿ
    [SerializeField] private bool isEquipSlot = false;

    private Item _item;
    private Player _player;
    private ItemSlotHandler slotHandler;
    private StatUI statUI;
    private WeaponHolder weaponHolder;

    public Item Item => _item;
    public ItemType? AllowedItemType => allowedItemType;

    public void Initialize(Player player, InventoryUI inventory, StatUI statUI)
    {
        _player = player;
        itemIcon.gameObject.SetActive(false);
        this.statUI = statUI;
        weaponHolder = player.GetComponent<WeaponHolder>();
        slotHandler = GetComponent<ItemSlotHandler>();
        if (slotHandler != null)
        {
            InitializeSlotHandler(inventory);
        }
    }

    private void InitializeSlotHandler(InventoryUI inventory)
    {
        slotHandler.IsInventorySlot = !isEquipSlot;

        slotHandler.InitializeSlotHandler(inventory);
    }

    public bool IsEmpty() => _item == null;

    public bool CanAcceptItem(Item item)
    {
        if (item == null) return false;

        if (isEquipSlot && allowedItemType.HasValue)
        {
            if (allowedItemType.Value == ItemType.Weapon)
            {
                return item.ItemType == ItemType.Sword || item.ItemType == ItemType.Bow;
            }

            return item.ItemType == allowedItemType.Value;
        }

        return true;
    }

    public void PlaceItem(Item item)
    {
        if (item != null && !CanAcceptItem(item)) return;

        RemoveCurrentItem();

        _item = item;

        if (_item != null)
        {
            itemIcon.sprite = _item.Icon;
            itemIcon.gameObject.SetActive(true);

            if (isEquipSlot && _player != null)
            {
                _item.ApplyStats(_player.GetComponent<ICharacterStats>());
                _player.UpdateMoveSpeed();

                if (_item.ItemType == ItemType.Sword || _item.ItemType == ItemType.Bow)
                {
                    weaponHolder.EquipWeapon(_item.ItemData.ItemPrefab, _item.ItemType);

                    Skill defaultSkill = _item.ItemData.GetDefaultSkillForWeapon();
                    if (defaultSkill != null)
                    {
                        _player.skillController.DirectEquipSkill(defaultSkill, KeyCode.Mouse0);
                    }

                    _player.AnimController.PlayerAnimatorChange(_item.ItemType);
                }
            }
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
        }

        statUI.UpdateUI();
    }

    private void RemoveCurrentItem()
    {
        if (_item != null && isEquipSlot && _player != null)
        {
            _item.RemoveStats(_player.GetComponent<ICharacterStats>());
            _player.UpdateMoveSpeed();

            if (_item.ItemType == ItemType.Sword || _item.ItemType == ItemType.Bow)
            {
                _player.skillController.RemoveAllWeaponSkills();
                _player.AnimController.PlayerAnimatorChange(ItemType.None);
                weaponHolder.UnequipCurrentWeapon();
            }
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
        RemoveCurrentItem();
        _item = null;
        itemIcon.gameObject.SetActive(false);
        itemIcon.color = Color.white;
        statUI.UpdateUI();
    }

    public void SetAsEquipmentSlot(ItemType type)
    {
        isEquipSlot = true;
        allowedItemType = type;

        if (slotHandler != null)
        {
            slotHandler.IsInventorySlot = false;
        }
    }
}
