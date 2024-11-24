using UnityEngine;

public class ArmorItem : Item
{
    [SerializeField] private float maxHealthBonus = 50f;        // �ִ� ü�� ����
    [SerializeField] private float maxHealthPercentBonus = 10f; // �ִ� ü�� % ����
    [SerializeField] private float defenseBonus = 15f;         // ���� ����
    [SerializeField] private float defensePercentBonus = 15f;  // ���� % ����

    protected override void ApplyStats(ICharacterStats stats)
    {
        ApplyStatModifier(stats, StatType.MaxHealth, maxHealthBonus, StatModifierType.Flat);
        ApplyStatModifier(stats, StatType.MaxHealth, maxHealthPercentBonus, StatModifierType.PercentAdd);

        ApplyStatModifier(stats, StatType.Defense, defenseBonus, StatModifierType.Flat);
        ApplyStatModifier(stats, StatType.Defense, defensePercentBonus, StatModifierType.PercentAdd);
    }
}