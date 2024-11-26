using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillStat : MonoBehaviour
{
    [System.Serializable]
    public class StatInitializer
    {
        [Tooltip("스탯 종류")]
        public SkillStatType Type;
        [Tooltip("기본값")]
        public float BaseValue;
        [Tooltip("토글식 스탯인지(딸깍)")]
        public bool IsBoolStat;
        [Tooltip("레벨당 증가량 (고정)")]
        public float GrowthValue = 0f;
        [Tooltip("레벨당 증가율 (%)")]
        public float growthPercent = 0f;
        public float GrowthRate => growthPercent / 100f;
    }

    protected Dictionary<SkillStatType, Stat> skillStats = new Dictionary<SkillStatType, Stat>();
    protected int currentLevel = 1;

    protected virtual void Awake()
    {
        InitializeStats();
    }

    protected abstract StatInitializer[] GetInitialStats();

    protected virtual void InitializeStats()
    {
        skillStats.Clear();
        var initialStats = GetInitialStats();

        foreach (var statInit in initialStats)
        {
            if (statInit.IsBoolStat)
            {
                skillStats[statInit.Type] = new BoolStat(statInit.BaseValue > 0);
            }
            else
            {
                skillStats[statInit.Type] = new LevelableStat(
                    statInit.BaseValue,
                    statInit.GrowthValue,
                    statInit.GrowthRate
                );
            }
        }

        foreach (SkillStatType skillStatType in System.Enum.GetValues(typeof(SkillStatType)))
        {
            if (!skillStats.ContainsKey(skillStatType))
            {
                skillStats[skillStatType] = new FloatStat(0f);
                Debug.LogWarning($"스킬스탯 {skillStatType} 초기화 되지 않았습니다 0으로 초기화됩니다.");
            }
        }

        SetSkillLevel(currentLevel);
    }

    public virtual T GetStatValue<T>(SkillStatType skillStatType)
    {
        if (skillStats.TryGetValue(skillStatType, out Stat stat))
        {
            return (T)stat.Value;
        }
        Debug.LogWarning($"스탯 {skillStatType} 없음 !!!");
        return default(T);
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

    public Dictionary<SkillStatType, object> GetCurrentStats()
    {
        Dictionary<SkillStatType, object> currentStats = new Dictionary<SkillStatType, object>();
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

    public virtual void SetSkillLevel(int level)
    {
        currentLevel = level;
        foreach (var stat in skillStats.Values)
        {
            if (stat is LevelableStat levelableStat)
            {
                levelableStat.SetLevel(level);
            }
        }
    }
}
