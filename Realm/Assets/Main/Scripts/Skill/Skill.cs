using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] protected SkillStat skillStat;
    protected int skillLevel = 1;

    public virtual void LevelUp()
    {
        skillLevel++;
        // SkillStat에 레벨 파라미터로 넘기면 지가 알아해라
        skillStat.SetSkillLevel(skillLevel);
    }

    public virtual void SetLevel(int level)
    {
        skillLevel = Mathf.Max(1, level);
        skillStat.SetSkillLevel(skillLevel);
    }

    //스킬 사용 구현하십시오
    protected abstract void UseSkill();
}
