using UnityEngine;

public class WeaponItem : Item
{
    [SerializeField] private float attackBonus = 10f;         // 기본 공격력 증가
    [SerializeField] private float attackPercentBonus = 20f;  // 공격력 20% 증가

    protected override void ApplyStats(ICharacterStats stats)
    {
        //              캐릭터스탯,   스탯 종류    , 아이템 스탯 ,  덧/곱연산 확인
        ApplyStatModifier(stats, StatType.Damage, attackBonus, StatModifierType.Flat);

        ApplyStatModifier(stats, StatType.Damage, attackPercentBonus, StatModifierType.PercentAdd);
    }
}