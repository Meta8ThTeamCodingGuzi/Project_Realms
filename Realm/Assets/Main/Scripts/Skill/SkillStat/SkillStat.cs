using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStat : MonoBehaviour
{
    [System.Serializable]
    public class StatInitializer
    {
        public SkillStatType Type;
        public float BaseValue;
    }

    protected Dictionary<SkillStatType, Stat> skillStats = new Dictionary<SkillStatType, Stat>();

    protected virtual void Awake()
    {
        InitializeStats();
    }

    protected StatInitializer[] initialStats = new StatInitializer[] { };

    protected virtual void InitializeStats()
    {
        skillStats.Clear();

        foreach (var statInit in initialStats)
        {
            skillStats[statInit.Type] = new Stat(statInit.BaseValue);
        }

        foreach (SkillStatType skillStatType in System.Enum.GetValues(typeof(SkillStatType)))
        {
            if (!skillStats.ContainsKey(skillStatType))
            {
                skillStats[skillStatType] = new Stat(0f);
                Debug.LogWarning($"스킬스탯 {skillStatType} 초기화 되지 않았습니다 0으로 초기화됩니다.");
            }
        }
    }

    public virtual float GetStatValue(SkillStatType skillStatType)
    {
        if (skillStats.TryGetValue(skillStatType, out Stat stat))
        {
            return stat.Value;
        }
        Debug.LogWarning($"스탯 {skillStatType} 없음 !!!");
        return 0f;
    }

    public virtual void AddModifier(SkillStatType skillStatType, StatModifier modifier)
    {
        if (skillStats.TryGetValue(skillStatType, out Stat stat))
        {
            stat.AddModifier(modifier);
        }
    }

    public virtual void RemoveModifier(SkillStatType skillStatType, StatModifier modifier)
    {
        if (skillStats.TryGetValue(skillStatType, out Stat stat))
        {
            stat.RemoveModifier(modifier);
        }
    }

    public Dictionary<SkillStatType, float> GetCurrentStats()
    {
        Dictionary<SkillStatType, float> currentStats = new Dictionary<SkillStatType, float>();
        foreach (var stat in skillStats)
        {
            currentStats[stat.Key] = stat.Value.Value;
        }
        return currentStats;
    }

    public Stat GetStat(SkillStatType skillStatType)
    {
        if (skillStats.TryGetValue(skillStatType, out Stat stat))
        {
            return stat;
        }
        Debug.LogWarning($"Stat {skillStatType} not found!");
        return null;
    }
}
