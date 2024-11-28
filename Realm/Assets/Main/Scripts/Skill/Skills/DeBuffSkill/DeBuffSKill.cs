using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeBuffSKill : Skill
{
    private DeBuffSkillStat deBuffSkillStat;
    private Coroutine BuffCoroutine;
    private List<Monster> monsters =new List<Monster>();
    private bool isSkillActive = false;

    public override void Initialize()
    {
        base.Initialize();
        deBuffSkillStat = (DeBuffSkillStat)skillStat;
        deBuffSkillStat.InitializeStats();
    }

    protected override void UseSkill()
    {
        if(!isSkillActive)
        {
            isSkillActive=true;           
        }
        else if (isSkillActive)
        {
            isSkillActive = false;         
        }
    }
    protected virtual void StopSkill()
    {

    }
    protected virtual void SetDeBuff(StatType statType,float value,StatModifierType modType)
    {
        StatModifier statModifier =new StatModifier(value,modType,this,SourceType.Debuff);
        

    }
    protected virtual void RemoveDeBuff(StatType statType)
    {
        foreach (Monster monster in monsters)
        {
            monster.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSkillActive || other == null) return;
        if(other.TryGetComponent<Monster>(out Monster monster))
        {
            if (monster != null && !monsters.Contains(monster))
            {
                monsters.Add(monster);
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other != null && other.TryGetComponent<Monster>(out Monster monster))
        {
            if(monster != null && monsters.Contains(monster))
            {
                monsters.Remove(monster);
            }
            
        }
    }


    protected virtual void OnDisable()
    {
         Debug.LogWarning($"{this}이거 OnDisable 구현 안했다.");
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }


}
