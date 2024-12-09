using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Debuff : MonoBehaviour
{
    private DeBuffSkillStat deBuffSkillStat;
    private List<Unit> debuffTargets = new List<Unit>();
    [SerializeField] protected StatType statType;
    [SerializeField] protected StatModifierType modifierType;
    private bool isOwnerPlayer = false;
    private Unit owner;

    public void Initialize(Unit owner,SkillStat skillStat)
    {
        this.owner = owner;
        deBuffSkillStat = (DeBuffSkillStat)skillStat;
        deBuffSkillStat.InitializeStats();
        this.transform.localScale = Vector3.one * deBuffSkillStat.GetStatValue<float>(SkillStatType.DeBuffAreaScale);
        isOwnerPlayer = owner is Player;
    }



    private void OnTriggerEnter(Collider other)
    {
        print("�ù� ��¥");
        if (other.TryGetComponent<Unit>(out Unit targetUnit))
        {
            print("�ù� ��¥2");
            if ((isOwnerPlayer && targetUnit is Monster) || (!isOwnerPlayer && targetUnit is Player))
            {
                print("�ù� ��¥3");
                debuffTargets.Add(targetUnit);
                SetDeBuff(targetUnit, statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
                print($"{targetUnit} ���� ����� ����");
                print($"{targetUnit.CharacterStats.GetStatValue(statType)} ������� ����");
            }
        }  
    }

    private void OnTriggerExit(Collider other)
    {
        print("�ù� ��¥4");
        if (other.TryGetComponent<Unit>(out Unit targetUnit))
        {
            print("�ù� ��¥5");
            if ((isOwnerPlayer && targetUnit is Monster) || (!isOwnerPlayer && targetUnit is Player))
            {
                print("�ù� ��¥6");
                RemoveDeBuff(targetUnit, statType);
                if (debuffTargets.Contains(targetUnit))
                {
                    debuffTargets.Remove(targetUnit);
                }
                print($"{targetUnit} ���� ����� ����");
            }
        }
    }

    protected virtual void OnDisable()
    {
        foreach (Unit debuffTarget in debuffTargets)
        {
            if (debuffTargets != null)
            {
                RemoveDeBuff(debuffTarget, statType);
            }
        }
        debuffTargets.Clear();
    }


    protected virtual void SetDeBuff(Unit target, StatType statType, float value, StatModifierType modType)
    {
        StatModifier statModifier = new StatModifier(-value, modType, this, SourceType.Debuff);
        target.CharacterStats.AddModifier(statType, statModifier);
        target.UpdateMoveSpeed();
    }

    protected virtual void RemoveDeBuff(Unit target, StatType statType)
    {
        target.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        target.UpdateMoveSpeed();
        print($"{target.CharacterStats.GetStatValue(statType)} ����������� ����");
    }
}
