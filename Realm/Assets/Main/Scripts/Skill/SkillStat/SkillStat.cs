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
        print("��ų ���� �ʱ�ȭ ȣ��");

        skillStats.Clear();
        var initialStats = GetInitialStats();

        foreach (var statInit in initialStats)
        {
            object baseValue = statInit.Type == SkillStatType.SkillLevel ?
                Mathf.RoundToInt(statInit.BaseValue) : statInit.BaseValue;

            skillStats[statInit.Type] = new LevelableStat(
                baseValue,
                statInit.GrowthValue,
                statInit.GrowthRate
            );

            if (statInit.Type == SkillStatType.SkillLevel)
            {
                print($"���緹�� ������ : {Mathf.RoundToInt(statInit.BaseValue)}");
            }
        }

        if (skillStats.TryGetValue(SkillStatType.SkillLevel, out Stat levelStat))
        {
            SetSkillLevel((int)levelStat.Value);
        }
    }

    public virtual T GetStatValue<T>(SkillStatType skillStatType)
    {
        if (skillStats.TryGetValue(skillStatType, out Stat stat))
        {
            return (T)stat.Value;
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
