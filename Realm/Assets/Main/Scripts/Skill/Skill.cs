using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillData data;
    [SerializeField] public SkillStat skillStat;

    public virtual void Initialize()
    {
        print("��ų �ʱ�ȭ �Լ� ȣ��");
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
        // SkillStat�� ���� �Ķ���ͷ� �ѱ�� ���� �˾��ض�
        skillStat.SetSkillLevel(skillStat.GetStatValue<int>(SkillStatType.SkillLevel));
    }

    public virtual void SetLevel(int level)
    {
        skillStat.SetSkillLevel(level);
    }

    //��ų ��� �����Ͻʽÿ�
    protected abstract void UseSkill();
}
