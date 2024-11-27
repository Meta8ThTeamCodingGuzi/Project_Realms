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
        [Tooltip("정수형 스탯인지")]
        public bool IsIntStat;
        [Tooltip("레벨당 증가량 (고정)")]
        public float GrowthValue = 0f;
        [Tooltip("레벨당 증가율 (%)")]
        public float growthPercent = 0f;
        public float GrowthRate => growthPercent / 100f;
    }

    protected Dictionary<SkillStatType, Stat> skillStats = new Dictionary<SkillStatType, Stat>();

    public int currentLevel = 0;

    protected abstract StatInitializer[] GetInitialStats();

    public virtual void InitializeStats()
    {
        print("스킬 스탯 초기화 호출");

        skillStats.Clear();
        var initialStats = GetInitialStats();

        foreach (var statInit in initialStats)
        {
            if (statInit.IsBoolStat)
            {
                skillStats[statInit.Type] = new BoolStat(statInit.BaseValue > 0);
            }
            if(statInit.IsIntStat) 
            {
                skillStats[statInit.Type] = new IntStat(Mathf.RoundToInt(statInit.BaseValue));
                if (statInit.Type == SkillStatType.SkillLevel)
                {
                    currentLevel = Mathf.RoundToInt(statInit.BaseValue);
                    print($"현재레벨 셋팅함 : {currentLevel}");
                }
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
        foreach (var stat in skillStats.Values)
        {
            if (stat is LevelableStat levelableStat)
            {
                levelableStat.SetLevel(level);
            }
        }
    }
}
