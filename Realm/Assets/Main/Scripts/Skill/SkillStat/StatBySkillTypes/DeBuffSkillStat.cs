using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeBuffSkillStat : SkillStat
{
    [Header("���� ��ų ����")]
    [SerializeField]
    private StatInitializer[] initialStats = new StatInitializer[]
    {
        new StatInitializer()
        {
            Type = SkillStatType.BuffValue,
        },
        new StatInitializer
        {
            Type= SkillStatType.ManaCost
        },
        new StatInitializer()
        {
            Type = SkillStatType.Cooldown,
        },
        new StatInitializer()
        {
            Type= SkillStatType.Duration,
        },
        new StatInitializer() 
        {
            Type = SkillStatType.DeBuffAreaScale,
        }
    };
    protected override StatInitializer[] GetInitialStats()
    {
        foreach (var stat in initialStats)
        {
            Debug.Log($"Stat: {stat.Type}, BaseValue: {stat.BaseValue}");
        }
        return initialStats;
    }

}
