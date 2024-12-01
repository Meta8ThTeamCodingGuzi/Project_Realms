using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    public ItemID ItemID => itemData.ItemID;
    public ItemType ItemType => itemData.ItemType;
    public GameObject ItemPrefab => itemData.ItemPrefab;
    public string Description => itemData.Description;
    public Sprite Icon => itemData.Icon;
    public IReadOnlyList<ItemData.ItemStat> Stats => itemData.Stats;
    public ItemData ItemData => itemData;

    protected virtual void ApplyStatModifier(ICharacterStats stats, StatType statType, float value, StatModifierType modType)
    {
        stats.AddModifier(statType, new StatModifier(value, modType, this, SourceType.Equipment));
    }

    public void ApplyStats(ICharacterStats characterStats)
    {
        foreach (var stat in Stats)
        {
            if (stat.flatValue != 0)
                ApplyStatModifier(characterStats, stat.statType, stat.flatValue, StatModifierType.Flat);
            if (stat.percentValue != 0)
                ApplyStatModifier(characterStats, stat.statType, stat.percentValue, StatModifierType.PercentAdd);
        }
    }

    public void RemoveStats(ICharacterStats characterStats)
    {
        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            characterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        }
    }

    public virtual string GetTooltip() => itemData.GetTooltip();
}