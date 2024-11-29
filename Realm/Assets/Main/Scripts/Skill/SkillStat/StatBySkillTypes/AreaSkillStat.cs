using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSkillStat : SkillStat
{
    [Header("���ǽ�ų ����")]
    [SerializeField]
    private StatInitializer[] initialStats = new StatInitializer[]
    {

        new StatInitializer
        {
            Type = SkillStatType.Damage,
        },
        new StatInitializer 
        {
            Type = SkillStatType.Duration,
        },
        new StatInitializer
        {
            Type = SkillStatType.SpawnRange,
        },
        new StatInitializer
        {
            Type = SkillStatType.SpawnScale,
        },
        new StatInitializer
        {
            Type = SkillStatType.SpawnCount,
        },
        new StatInitializer
        {
            Type = SkillStatType.SpawnInterval,
        },
        new StatInitializer
        {
            Type = SkillStatType.IsSpawnAtCursor,
        },
        new StatInitializer
        {
            Type = SkillStatType.IsSpawnAtEnemy,
        },

    };

    protected override StatInitializer[] GetInitialStats()
    {
        return initialStats;
    }

}

