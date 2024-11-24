using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected string itemName = "기본 아이템";
    [SerializeField] protected string description = "아이템 설명";
    protected bool isEquipped;

    public string ItemName => itemName;
    public string Description => description;
    public bool IsEquipped => isEquipped;

    protected virtual void ApplyStatModifier(ICharacterStats stats, StatType statType, float value, StatModifierType modType)
    {
        stats.AddModifier(statType, new StatModifier(value, modType, this, SourceType.Equipment));
    }

    public virtual bool CanEquip(Player player)
    {
        return !isEquipped && player != null && player.IsAlive;
    }

    public virtual void Equip(Player player)
    {
        if (!CanEquip(player)) return;

        ICharacterStats stats = player.GetComponent<ICharacterStats>();
        if (stats != null)
        {
            ApplyStats(stats);
            isEquipped = true;
            OnEquipped();
        }
    }

    public virtual void Unequip(Player player)
    {
        if (!isEquipped || player == null) return;

        ICharacterStats stats = player.GetComponent<ICharacterStats>();
        if (stats != null)
        {
            foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
            {
                stats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
            }
            isEquipped = false;
            OnUnequipped();
        }
    }

    protected abstract void ApplyStats(ICharacterStats stats);

    protected virtual void OnEquipped()
    {
        Debug.Log($"{itemName}을(를) 장착했습니다.");
    }

    protected virtual void OnUnequipped()
    {
        Debug.Log($"{itemName}을(를) 해제했습니다.");
    }
}