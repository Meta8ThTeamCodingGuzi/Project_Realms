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
        //  Ÿ
        slotHandler.IsInventorySlot = !isEquipSlot;

        // κ丮  (Կ  )
        slotHandler.InitializeSlotHandler(inventory);
    }

    public bool IsEmpty() => _item == null;

    public bool CanAcceptItem(Item item)
    {
        if (item == null) return false;

        // 장비 슬롯이면서 허용된 타입이 있는 경우
        if (isEquipSlot && allowedItemType.HasValue)
        {
            // Weapon 타입 슬롯은 Sword와 Bow를 모두 허용
            if (allowedItemType.Value == ItemType.Weapon)
            {
                return item.ItemType == ItemType.Sword || item.ItemType == ItemType.Bow;
            }

            return item.ItemType == allowedItemType.Value;
        }

        // 인벤토리 슬롯은 모든 아이템 허용
        return true;
    }

    public void PlaceItem(Item item)
    {
        if (item != null && !CanAcceptItem(item)) return;

        Item oldItem = _item;  
        _item = null;  

        if (isEquipSlot && oldItem != null && _player != null)
        {
            oldItem.RemoveStats(_player.GetComponent<ICharacterStats>());

            if (oldItem.ItemType == ItemType.Sword || oldItem.ItemType == ItemType.Bow)
            {
                _player.skillController.UnequipSkill(KeyCode.Mouse0);
                _player.PlayerAnimController.AnimatorChange(ItemType.None);

                var weaponHolder = _player.GetComponent<WeaponHolder>();
                weaponHolder.UnequipCurrentWeapon();
            }
        }

        _item = item;

        if (_item != null)
        {
            itemIcon.sprite = _item.Icon;
            itemIcon.gameObject.SetActive(true);

            if (isEquipSlot && _player != null)
            {
                _item.ApplyStats(_player.GetComponent<ICharacterStats>());

                if (_item.ItemType == ItemType.Sword || _item.ItemType == ItemType.Bow)
                {
                    var weaponHolder = _player.GetComponent<WeaponHolder>();
                    weaponHolder.EquipWeapon(_item.ItemData.ItemPrefab, _item.ItemType);

                    Skill defaultSkill = _item.ItemData.GetDefaultSkillForWeapon();
                    if (defaultSkill != null)
                    {
                        _player.skillController.AddSkill(defaultSkill);
                        _player.skillController.EquipSkill(defaultSkill, KeyCode.Mouse0);

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

        // StatUI 업데이트
        statUI.UpdateUI();
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

        // ڵ鷯 Ʈ
        if (slotHandler != null)
        {
            slotHandler.IsInventorySlot = false;
        }
    }
}
