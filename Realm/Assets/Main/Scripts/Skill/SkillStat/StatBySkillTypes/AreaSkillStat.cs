using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSkillStat : SkillStat
{
    [Header("장판스킬 스텟")]
    [SerializeField]
    private StatInitializer[] initialStats = new StatInitializer[]
    {

        new StatInitializer
        {
            Type =SkillStatType.Damage,
            BaseValue = 5f,
            GrowthValue =5f,
            growthPercent =10f
        },
        new StatInitializer {
            Type =SkillStatType.Duration,
            BaseValue = 5f,
            GrowthValue = 1f
        },
        new StatInitializer
        {
            Type= SkillStatType.ProjectileRange,
            BaseValue = 10f,
            GrowthValue =0f
        },
        new StatInitializer
        {
            Type = SkillStatType.ProjectileScale,
            BaseValue = 5f,
            GrowthValue= 0.1f
        },
        new StatInitializer
        {
            Type = SkillStatType.ProjectileCount,
            BaseValue = 1f,
            GrowthValue = 0f
        },
        new StatInitializer
        {
            Type = SkillStatType.Cooldown,
            BaseValue =7f,
            GrowthValue = 0f
        },
        new StatInitializer
        {
            Type = SkillStatType.InnerInterval,
            BaseValue = 1f,
            GrowthValue = 0f
        },
        new StatInitializer
        {
            Type =SkillStatType.IsTraceMouse,
            IsBoolStat = true,
            GrowthValue = 0f
        }

    };

        protected override StatInitializer[] GetInitialStats()
    {
        return initialStats;
    }

}

