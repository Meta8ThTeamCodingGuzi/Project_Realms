using UnityEngine;

public class ArmorItem : Item
{
    [SerializeField] private float maxHealthBonus = 50f;        // 최대 체력 증가
    [SerializeField] private float maxHealthPercentBonus = 10f; // 최대 체력 % 증가
    [SerializeField] private float defenseBonus = 15f;         // 방어력 증가
    [SerializeField] private float defensePercentBonus = 15f;  // 방어력 % 증가

    protected override void ApplyStats(ICharacterStats stats)
    {
        ApplyStatModifier(stats, StatType.MaxHealth, maxHealthBonus, StatModifierType.Flat);
        ApplyStatModifier(stats, StatType.MaxHealth, maxHealthPercentBonus, StatModifierType.PercentAdd);

        ApplyStatModifier(stats, StatType.Defense, defenseBonus, StatModifierType.Flat);
        ApplyStatModifier(stats, StatType.Defense, defensePercentBonus, StatModifierType.PercentAdd);
    }
}