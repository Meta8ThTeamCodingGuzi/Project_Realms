using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashStat : SkillStat
{

    [Header("�뽬 ��ų ����")]
    [SerializeField]
    private StatInitializer[] initialStats = new StatInitializer[]
{
        new StatInitializer()
        {
            Type = SkillStatType.SkillLevel,
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
            Type= SkillStatType.DashDistance,
        }
};




    protected override StatInitializer[] GetInitialStats()
    {
        Debug.Log($"initialStats count: {initialStats.Length}");
        foreach (var stat in initialStats)
        {
            Debug.Log($"Stat: {stat.Type}, BaseValue: {stat.BaseValue}");
        }
        return initialStats;
    }
}
