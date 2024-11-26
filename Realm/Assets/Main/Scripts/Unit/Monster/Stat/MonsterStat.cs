using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : UnitStats
{
    [Header("�̰Թ��κ��� ���� �������ݾ�")]
    [SerializeField]
    private StatInitializer[] initialStats = new StatInitializer[]
    {
        new StatInitializer()
        {
            Type = StatType.Level,
            BaseValue = 1f,
            PointIncrease = 0f,
        },
        new StatInitializer()
        {
            Type= StatType.MoveSpeed,
            BaseValue= 4f,
            PointIncrease = 0.2f,
        },
        new StatInitializer()
        {
            Type = StatType.Defense,
            BaseValue = 10f,
            PointIncrease = 2f,
        },
        new StatInitializer()
        {
            Type = StatType.DropExp,
            BaseValue= 20f,
            PointIncrease = 3f,
        },
        new StatInitializer()
        {
            Type = StatType.AttackSpeed,
            BaseValue = 1.5f,
            PointIncrease =0f,
        },
        new StatInitializer()
        {
            Type = StatType.AttackRange,
            BaseValue = 2f,
            PointIncrease =0f,

        },
        new StatInitializer()
        {
            Type = StatType.Damage,
            BaseValue = 50f,
            PointIncrease=20f,
        },
        new StatInitializer()
        {
            Type = StatType.MaxHealth,
            BaseValue = 250f,
            PointIncrease = 50f,
        },
    };

    protected override StatInitializer[] GetInitialStats()
    {
        return initialStats;
    }
}
