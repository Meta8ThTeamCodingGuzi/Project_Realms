using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillStat : MonoBehaviour
{
    [System.Serializable]
    public class StatInitializer
    {
        [Tooltip("���� ����")]
        public SkillStatType Type;
        [Tooltip("�⺻��")]
        public float BaseValue;
        [Tooltip("������ ������ (����)")]
        public float GrowthValue = 0f;
        [Tooltip("������ ������ (%)")]
        public float growthPercent = 0f;
        public float GrowthRate => growthPercent / 100f;
    }

    protected Dictionary<SkillStatType, Stat> skillStats = new Dictionary<SkillStatType, Stat>();

    protected abstract StatInitializer[] GetInitialStats();

    public virtual void InitializeStats()
    {

        skillStats.Clear();
        var initialStats = GetInitialStats();

        foreach (var statInit in initialStats)
        {
            bool isInteger = Mathf.Approximately(statInit.BaseValue, Mathf.Round(statInit.BaseValue));

            if (isInteger)
            {
                skillStats[statInit.Type] = new LevelableStat(
                    (int)statInit.BaseValue,
                    (int)statInit.GrowthValue,
                    statInit.GrowthRate
                );
                //print($"{statInit.Type} ���� �ʱ�ȭ: {(int)statInit.BaseValue} (����)");
            }
            else
            {
                skillStats[statInit.Type] = new LevelableStat(
                    statInit.BaseValue,
                    statInit.GrowthValue,
                    statInit.GrowthRate
                );
                //print($"{statInit.Type} ���� �ʱ�ȭ: {statInit.BaseValue} (�Ǽ�)");
            }
        }

        SetSkillLevel(GetStatValue<int>(SkillStatType.SkillLevel));
        
    }

    public virtual T GetStatValue<T>(SkillStatType skillStatType)
    {
        if (skillStats.TryGetValue(skillStatType, out Stat stat))
        {
            if (stat.Value is T value)
            {
                return value;
            }

            if (typeof(T) == typeof(int) && stat.Value is float floatValue)
            {
                return (T)(object)Mathf.RoundToInt(floatValue);
            }

            if (typeof(T) == typeof(float) && stat.Value is int intValue)
            {
                return (T)(object)((float)intValue);
            }

            Debug.LogWarning($"���� {skillStatType}�� ���� {typeof(T)}�� ��ȯ�� �� �����ϴ�!");
            return default(T);
        }
        Debug.LogWarning($"���� {skillStatType} ���� !!!");
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
        if (level < 1) level = 1;

        if (skillStats.TryGetValue(SkillStatType.SkillLevel, out Stat levelStat))
        {
            if (levelStat is LevelableStat levelableStat)
            {
                levelableStat.SetLevel(level);
            }
        }

        foreach (var stat in skillStats.Values)
        {
            if (stat is LevelableStat levelableStat && stat != levelStat)
            {
                levelableStat.SetLevel(level);
            }
        }
    }


}
