using UnityEngine;
using UnityEngine.UI;


public class Slot : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private ItemType? allowedItemType = null; // null이면 모든 타입 허용
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
        // 장비 슬롯인지 여부 설정
        slotHandler.IsInventorySlot = !isEquipSlot;

        // 인벤토리 참조 설정 (모든 슬롯에 대해 설정)
        slotHandler.InitializeSlotHandler(inventory);
    }

    public bool IsEmpty() => _item == null;

    public bool CanAcceptItem(Item item)
    {
        if (item == null) return false;

        // 장비 슬롯이면 아이템 타입 체크
        if (isEquipSlot && allowedItemType.HasValue)
        {
            return item.ItemType == allowedItemType.Value;
        }

        // 인벤토리 슬롯은 모든 아이템 허용
        return true;
    }

    public void PlaceItem(Item item)
    {
        if (item != null && !CanAcceptItem(item)) return;

        // 이전 아이템의 효과 제거
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

            // 새 아이템 효과 적용
            if (isEquipSlot && _player != null)
            {
                _item.ApplyStats(_player.GetComponent<ICharacterStats>());
                statUI.UpdateUI();
                // TODO : 아이템 착용시 플레이어 아이템 장착 위치 아래로 아이템 소환 로직
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

        // 슬롯 핸들러 설정 업데이트
        if (slotHandler != null)
        {
            slotHandler.IsInventorySlot = false;
        }
    }
}
