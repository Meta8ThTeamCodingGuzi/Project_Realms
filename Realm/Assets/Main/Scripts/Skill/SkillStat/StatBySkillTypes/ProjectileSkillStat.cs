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
            Type = SkillStatType.Damage,
            BaseValue = 10f,
            GrowthValue = 5f,      // ������ ������ ����
            growthPercent = 10f    // ������ % ����
        },
        new StatInitializer 
        {
            Type = SkillStatType.ProjectileSpeed,
            BaseValue = 5f,
            GrowthValue = 0.5f    
        },
        new StatInitializer 
        {
            Type = SkillStatType.ProjectileRange,
            BaseValue = 5f,
            growthPercent = 5f    
        },
        new StatInitializer 
        {
            Type = SkillStatType.PierceCount,
            BaseValue = 0f,
            GrowthValue = 1f     
        },
        new StatInitializer 
        {
            Type = SkillStatType.ExplosionRadius,
            BaseValue = 0f,
            GrowthValue = 0.2f   
        },
        new StatInitializer 
        {
            Type = SkillStatType.HomingRange,
            BaseValue = 0f,
            GrowthValue = 0.5f   
        },
        new StatInitializer 
        {
            Type = SkillStatType.InnerInterval,
            BaseValue = 0f,
            GrowthValue = 0f  
        },
        new StatInitializer 
        {
            Type = SkillStatType.ShotInterval,
            BaseValue = 0f,
            GrowthValue = 0f   
        },                       
        new StatInitializer 
        {
            Type = SkillStatType.ProjectileCount,
            BaseValue = 0f,
            GrowthValue = 1f    
        },
        new StatInitializer 
        {
            Type = SkillStatType.IsHoming,
            BaseValue = 0f,
            IsBoolStat = true
        },
    };

    protected override StatInitializer[] GetInitialStats()
    {
        return initialStats;
    }
}
