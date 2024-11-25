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
        // SkillStat�� ���� �Ķ���ͷ� �ѱ�� ���� �˾��ض�
        skillStat.SetSkillLevel(skillLevel);
    }

    public virtual void SetLevel(int level)
    {
        skillLevel = Mathf.Max(1, level);
        skillStat.SetSkillLevel(skillLevel);
    }

    //��ų ��� �����Ͻʽÿ�
    protected abstract void UseSkill();
}
