using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillData data;
    [SerializeField] public SkillStat skillStat;

    public virtual void Initialize()
    {
        print("스킬 초기화 함수 호출");
        if (skillStat == null)
        {
            skillStat = GetComponent<SkillStat>();
            if (skillStat == null)
            {
                Debug.LogError("SkillStat component not found!");
            }
        }
    }

    public virtual void LevelUp()
    {
        skillStat.AddModifier(SkillStatType.SkillLevel, new StatModifier(1, StatModifierType.Flat, SourceType.BaseStats));
        // SkillStat에 레벨 파라미터로 넘기면 지가 알아해라
        skillStat.SetSkillLevel(skillStat.GetStatValue<int>(SkillStatType.SkillLevel));
    }

    public virtual void SetLevel(int level)
    {
        skillStat.SetSkillLevel(level);
    }

    //스킬 사용 구현하십시오
    protected abstract void UseSkill();
}
