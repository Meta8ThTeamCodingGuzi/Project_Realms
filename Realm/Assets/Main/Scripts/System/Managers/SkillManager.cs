using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : SingletonManager<SkillManager>
{
    public List<Skill> allSkillsPrefabs;

    public Skill GetSkill(SkillID skillID)
    {
        foreach (Skill skill in allSkillsPrefabs)
        {
            if (skill.data.skillID == skillID)
            {
                return skill;
            }
        }
        Debug.LogError($"��ų�Ŵ����� {skillID} ��ų �����");
        return null;
    }

}
