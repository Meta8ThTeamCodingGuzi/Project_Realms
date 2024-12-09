using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class Debuff : MonoBehaviour
{
    private DeBuffSkillStat deBuffSkillStat;
    private List<Monster> monsters = new List<Monster>();
    [SerializeField] protected StatType statType;
    [SerializeField] protected StatModifierType modifierType;
    private Unit owner;

    public void Initialize(Unit owner,SkillStat skillStat)
    {
        this.owner = owner;
        deBuffSkillStat = (DeBuffSkillStat)skillStat;
        deBuffSkillStat.InitializeStats();
        this.transform.localScale = Vector3.one * deBuffSkillStat.GetStatValue<float>(SkillStatType.DeBuffAreaScale);
    }



    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Monster>(out Monster mon))
        {
            print("몬스터 닿음");        }

        if (owner.TryGetComponent<Monster>(out Monster monster)) 
        {
            if (other.TryGetComponent<Player>(out Player player)) 
            {
                if (player != null) 
                {
                    SetDeBuff(player,statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
                    print("몬스터가 플레이어에게 디버프 적용");
                }
            }
        }
        if (owner.TryGetComponent<Player>(out Player Player))
        {
            if (other.TryGetComponent<Monster>(out Monster Monster)) 
            {
                if (monster != null && !monsters.Contains(monster))
                {
                    monsters.Add(monster);
                    SetDeBuff(monster, statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
                    print("플레이어가 몬스터에게 디버프 적용");
                }
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (owner.TryGetComponent<Monster>(out Monster monster))
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                if (player != null)
                {
                    RemoveDeBuff(player, statType);
                    print("플레이어 디버프 해제");
                }
            }
        }
        if (owner.TryGetComponent<Player>(out Player Player))
        {
            if (other != null && other.TryGetComponent<Monster>(out Monster Monster))
            {
                if (Monster != null && monsters.Contains(Monster))
                {
                    RemoveDeBuff(Monster, statType);
                    monsters.Remove(Monster);
                }
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


    protected virtual void SetDeBuff(Unit target, StatType statType, float value, StatModifierType modType)
    {
        StatModifier statModifier = new StatModifier(value, modType, this, SourceType.Debuff);
        target.CharacterStats.AddModifier(statType, statModifier);
        target.UpdateMoveSpeed();
    }

    protected virtual void RemoveDeBuff(Unit target, StatType statType)
    {
        target.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        target.UpdateMoveSpeed();
        print("디버프 해제");
    }
}
