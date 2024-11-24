using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour, ICharacterStats
{
    [System.Serializable]
    public class StatInitializer
    {
        public StatType Type;
        public float BaseValue;
    }

    [Header("Base Stats")]
    [SerializeField]
    protected StatInitializer[] initialStats = new StatInitializer[]
    {
        new StatInitializer { Type = StatType.MaxHealth, BaseValue = 100f },
        new StatInitializer { Type = StatType.Health, BaseValue = 100f },
        new StatInitializer { Type = StatType.Attack, BaseValue = 10f },
        new StatInitializer { Type = StatType.Defense, BaseValue = 10f },
        new StatInitializer { Type = StatType.MoveSpeed, BaseValue = 5f },
        new StatInitializer { Type = StatType.AttackSpeed, BaseValue = 1f },
        new StatInitializer { Type = StatType.AttackRange, BaseValue = 2f }
    };

    protected Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    protected virtual void Awake()
    {
        InitializeStats();
    }

    protected virtual void InitializeStats()
    {
        stats.Clear();

        foreach (var statInit in initialStats)
        {
            stats[statInit.Type] = new Stat(statInit.BaseValue);

            if (statInit.Type == StatType.MaxHealth)
            {
                stats[StatType.Health] = new Stat(statInit.BaseValue);
            }
        }

        foreach (StatType statType in System.Enum.GetValues(typeof(StatType)))
        {
            if (!stats.ContainsKey(statType))
            {
                stats[statType] = new Stat(0f);
                Debug.LogWarning($"Stat {statType} was not initialized. Setting to 0.");
            }
        }
    }

    public virtual float GetStatValue(StatType statType)
    {
        if (stats.TryGetValue(statType, out Stat stat))
        {
            return stat.Value;
        }
        Debug.LogWarning($"Stat {statType} not found!");
        return 0f;
    }

    public virtual void AddModifier(StatType statType, StatModifier modifier)
    {
        if (stats.TryGetValue(statType, out Stat stat))
        {
            float oldMaxHealth = 0;
            if (statType == StatType.MaxHealth)
            {
                oldMaxHealth = stat.Value;
            }

            stat.AddModifier(modifier);

            if (statType == StatType.MaxHealth)
            {
                float newMaxHealth = stat.Value;
                if (oldMaxHealth > 0)
                {
                    float healthRatio = GetStatValue(StatType.Health) / oldMaxHealth;
                    float newHealth = newMaxHealth * healthRatio;

                    if (stats.TryGetValue(StatType.Health, out Stat healthStat))
                    {
                        foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
                        {
                            healthStat.RemoveAllModifiersFromSource(modifier.Source);
                        }
                        healthStat.BaseValue = newHealth;
                    }
                }
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
            currentStats[stat.Key] = stat.Value.Value;
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
