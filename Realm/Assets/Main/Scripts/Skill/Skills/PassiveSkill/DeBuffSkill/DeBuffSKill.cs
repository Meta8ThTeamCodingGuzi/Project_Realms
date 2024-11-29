using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DebuffSKill : Skill
{
    private DeBuffSkillStat deBuffSkillStat;
    private List<Monster> monsters = new List<Monster>();
    [SerializeField] protected StatType statType;
    [SerializeField] protected StatModifierType modifierType;
    private bool isSkillActive = true;


    public override void Initialize()
    {
        base.Initialize();
        deBuffSkillStat = (DeBuffSkillStat)skillStat;
        deBuffSkillStat.InitializeStats();
        this.transform.localScale = Vector3.one * deBuffSkillStat.GetStatValue<float>(SkillStatType.DeBuffAreaScale);
    }

    protected override void UseSkill()
    {
        if (!isSkillActive)
        {
            isSkillActive = true;
        }
        else if (isSkillActive)
        {
            isSkillActive = false;
        }
    }

    protected virtual void StopSkill()
    {

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isSkillActive || other == null) return;
        if (other.TryGetComponent<Monster>(out Monster monster))
        {
            if (monster != null && !monsters.Contains(monster))
            {
                monsters.Add(monster);
                SetDeBuff(monster, statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
                print("디버프 적용");
            }

        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.TryGetComponent<Monster>(out Monster monster))
        {
            if (monster != null && monsters.Contains(monster))
            {
                RemoveDeBuff(monster, statType);
                monsters.Remove(monster);
            }
        }
    }


    protected virtual void OnDisable()
    {
        foreach (Monster monster in monsters)
        {
            RemoveDeBuff(monster, statType);
        }
        monsters.Clear();
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    #region 버프적용 버프제거 로직
    protected virtual void SetDeBuff(Monster monster, StatType statType, float value, StatModifierType modType)
    {
        StatModifier statModifier = new StatModifier(value, modType, this, SourceType.Debuff);
        monster.CharacterStats.AddModifier(statType, statModifier);
        monster.UpdateMoveSpeed();
    }

    protected virtual void RemoveDeBuff(Monster monster, StatType statType)
    {
        monster.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        monster.UpdateMoveSpeed();
        print("디버프 해제");
    }
    #endregion

}
