using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkillStat : SkillStat
{
    [Header("����ü ��ų ����")]
    [SerializeField]
    private StatInitializer[] initialStats = new StatInitializer[]
    {
        new StatInitializer
        {
            Type = SkillStatType.SkillLevel,
        },
        new StatInitializer
        {
            Type= SkillStatType.ManaCost
        },
        new StatInitializer
        {
            Type = SkillStatType.HomingLevel,
        },
        new StatInitializer 
        {
            Type = SkillStatType.Damage,
        },
        new StatInitializer 
        {
            Type = SkillStatType.ProjectileSpeed,
        },
        new StatInitializer 
        {
            Type = SkillStatType.ProjectileRange,  
        },
        new StatInitializer 
        {
            Type = SkillStatType.PierceCount,
        },
        new StatInitializer 
        {
            Type = SkillStatType.ExplosionRadius,
        },
        new StatInitializer 
        {
            Type = SkillStatType.HomingRange,  
        },
        new StatInitializer 
        {
            Type = SkillStatType.InnerInterval,
        },
        new StatInitializer 
        {
            Type = SkillStatType.ShotInterval,  
        },                       
        new StatInitializer 
        {
            Type = SkillStatType.ProjectileCount,
        },
        new StatInitializer 
        {
            Type = SkillStatType.IsHoming,
        },
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
