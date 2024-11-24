using System.Collections.Generic;
using UnityEngine;

public abstract class UnitStats : MonoBehaviour, ICharacterStats
{
    [System.Serializable]
    public class StatInitializer
    {
        [Header("스탯 종류")]
        public StatType Type;
        [Header("기본값")]
        public float BaseValue;
        [Header("스탯포인트 하나당 얼마나 오를지") , Tooltip("만약 스탯 타입이 [레벨]일 경우 0으로 설정 부탁드립니다.")]
        public float PointIncrease = 1f;
    }

    protected Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    protected virtual void Awake()
    {
        InitializeStats();
    }

    protected abstract StatInitializer[] GetInitialStats();

    protected virtual void InitializeStats()
    {
        stats.Clear();

        var initialStats = GetInitialStats();

        foreach (var statInit in initialStats)
        {
            stats[statInit.Type] = new FloatStat(statInit.BaseValue, statInit.PointIncrease);

            if (statInit.Type == StatType.MaxHealth)
            {
                stats[StatType.Health] = new FloatStat(statInit.BaseValue, statInit.PointIncrease);
            }
        }

        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            if (!stats.ContainsKey(statType))
            {
                stats[statType] = new FloatStat(0f);
                Debug.LogWarning($"스탯 {statType} 초기화 되지 않았습니다 0으로 초기화됩니다.");
            }
        }
    }

    public virtual float GetStatValue(StatType statType)
    {
        if (stats.TryGetValue(statType, out Stat stat))
        {
            return (float)stat.Value;
        }
        Debug.LogWarning($"스탯 {statType} 없음 !!!");
        return 0f;
    }

    public virtual void AddModifier(StatType statType, StatModifier modifier)
    {
        if (stats.TryGetValue(statType, out Stat stat))
        {
            if (statType == StatType.MaxHealth)
            {
                float oldMaxHealth = (float)stat.Value;
                stat.AddModifier(modifier);
                float newMaxHealth = (float)stat.Value;

                if (oldMaxHealth > 0 && stats.TryGetValue(StatType.Health, out Stat healthStat))
                {
                    float healthRatio = (float)healthStat.Value / oldMaxHealth;
                    float newHealth = newMaxHealth * healthRatio;

                    healthStat.RemoveAllModifiersFromSource(modifier.Source);
                    ((FloatStat)healthStat).InvestPoint(newHealth - (float)healthStat.Value);
                }
            }
            else
            {
                stat.AddModifier(modifier);
            }
        }
    }

    public virtual void RemoveModifier(StatType statType, StatModifier modifier)
    {
        if (stats.TryGetValue(statType, out Stat stat))
        {
            stat.RemoveModifier(modifier);
        }
    }

    public Dictionary<StatType, float> GetCurrentStats()
    {
        Dictionary<StatType, float> currentStats = new Dictionary<StatType, float>();
        foreach (var stat in stats)
        {
            currentStats[stat.Key] = (float)stat.Value.Value;
        }
        return currentStats;
    }

    public Stat GetStat(StatType statType)
    {
        if (stats.TryGetValue(statType, out Stat stat))
        {
            return stat;
        }
        Debug.LogWarning($"Stat {statType} not found!");
        return null;
    }
}
