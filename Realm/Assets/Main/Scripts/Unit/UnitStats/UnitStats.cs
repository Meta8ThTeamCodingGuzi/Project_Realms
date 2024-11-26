using System.Collections.Generic;
using UnityEngine;

public abstract class UnitStats : MonoBehaviour, ICharacterStats
{
    [System.Serializable]
    public class StatInitializer
    {
        [Header("���� ����")]
        public StatType Type;
        [Header("�⺻��")]
        public float BaseValue;
        [Header("��������Ʈ �ϳ��� �󸶳� ������") , Tooltip("���� ���� Ÿ���� [����]�� ��� 0���� ���� ��Ź�帳�ϴ�.")]
        public float PointIncrease = 1f;
    }

    protected Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat>();

    protected abstract StatInitializer[] GetInitialStats();

    public virtual void InitializeStats()
    {
        stats.Clear();
        var initialStats = GetInitialStats();

        Debug.Log($"Initializing stats with {initialStats.Length} values");

        foreach (var statInit in initialStats)
        {
            Debug.Log($"Initializing {statInit.Type} with base value {statInit.BaseValue}");
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
                Debug.Log($"�� ��ü�� {statType} ���� �Ⱦ��� �����ΰ���?");
            }
        }
    }

    public virtual float GetStatValue(StatType statType)
    {
        if (stats.TryGetValue(statType, out Stat stat))
        {
            return (float)stat.Value;
        }
        Debug.LogWarning($"���� {statType} ���� !!!");
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
