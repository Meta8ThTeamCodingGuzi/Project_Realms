using UnityEngine;

public class WeaponItem : Item
{
    [SerializeField] private float attackBonus = 10f;         // �⺻ ���ݷ� ����
    [SerializeField] private float attackPercentBonus = 20f;  // ���ݷ� 20% ����

    protected override void ApplyStats(ICharacterStats stats)
    {
        //              ĳ���ͽ���,   ���� ����    , ������ ���� ,  ��/������ Ȯ��
        ApplyStatModifier(stats, StatType.Damage, attackBonus, StatModifierType.Flat);

        ApplyStatModifier(stats, StatType.Damage, attackPercentBonus, StatModifierType.PercentAdd);
    }
}