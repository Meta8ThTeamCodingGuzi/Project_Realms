using UnityEngine;

public class AccessoryItem : Item
{
    [SerializeField] private float moveSpeedBonus = 1f;          
    [SerializeField] private float moveSpeedPercentBonus = 10f;  

    protected override void ApplyStats(ICharacterStats stats)
    {
        ApplyStatModifier(stats, StatType.MoveSpeed, moveSpeedBonus, StatModifierType.Flat);
        ApplyStatModifier(stats, StatType.MoveSpeed, moveSpeedPercentBonus, StatModifierType.PercentAdd);
    }

    protected override void OnEquipped()
    {
        base.OnEquipped();
        Debug.Log($"이동속도 +{moveSpeedBonus} (+{moveSpeedPercentBonus}%)");
    }
}