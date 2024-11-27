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
        [Tooltip("��۽� ��������(����)")]
        public bool IsBoolStat;
        [Tooltip("������ ��������")]
        public bool IsIntStat;
        [Tooltip("������ ������ (����)")]
        public float GrowthValue = 0f;
        [Tooltip("������ ������ (%)")]
        public float growthPercent = 0f;
        public float GrowthRate => growthPercent / 100f;
    }

    protected Dictionary<SkillStatType, Stat> skillStats = new Dictionary<SkillStatType, Stat>();

    public int currentLevel = 0;

    protected abstract StatInitializer[] GetInitialStats();

    public virtual void InitializeStats()
    {
        print("��ų ���� �ʱ�ȭ ȣ��");

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
                    print($"���緹�� ������ : {currentLevel}");
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
        foreach (var stat in skillStats.Values)
        {
            if (stat is LevelableStat levelableStat)
            {
                levelableStat.SetLevel(level);
            }
        }
    }
}
