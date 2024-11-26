using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerStat : UnitStats
{
    [Header("�÷��̾� �⺻ ����")]
    [SerializeField]
    private StatInitializer[] initialStats = new StatInitializer[]
    {
        new StatInitializer
        {
            Type = StatType.Level,
            BaseValue = 1f,
            PointIncrease = 0f
        },
        new StatInitializer
        {
            Type = StatType.AttackRange,
            BaseValue = 1f,
            PointIncrease = 0.5f
        },
        new StatInitializer 
        {
            Type = StatType.AttackSpeed,
            BaseValue = 1f,
            PointIncrease = 0.5f
        },
        new StatInitializer
        {
            Type= StatType.Damage,
            BaseValue= 5f,
            PointIncrease = 3f
        },
        new StatInitializer
        {
            Type = StatType.MoveSpeed,
            BaseValue = 7f,
            PointIncrease = 0.5f    
        },
        new StatInitializer
        {
            Type = StatType.MaxHealth,
            BaseValue = 500f,
            PointIncrease = 0.5f    
        },
        new StatInitializer
        {
            Type = StatType.Defense,
            BaseValue = 20f,
            PointIncrease = 30f
        },
        new StatInitializer
        {
            Type = StatType.ExpRange,
            BaseValue = 3f,
            PointIncrease = 2f
        },
    };

    protected override StatInitializer[] GetInitialStats()
    {
        return initialStats;
    }
}

