using UnityEngine;
using System.Collections.Generic;

public class StatPointSystem : MonoBehaviour
{
    [System.Serializable]
    public class StatInvestSettings
    {
        public StatType StatType;
        public float IncreaseAmount = 1f;  // 포인트당 증가량
        public int MaxPoints = 100;         // 최대 투자 가능 포인트
    }

    [SerializeField] private UnitStats unitStats;
    [SerializeField] private StatInvestSettings[] statSettings;

    private Dictionary<StatType, StatInvestSettings> settingsMap = new Dictionary<StatType, StatInvestSettings>();
    public int AvailablePoints { get; private set; }

    private void Awake()
    {
        unitStats = GetComponent<UnitStats>();
        foreach (var setting in statSettings)
        {
            settingsMap[setting.StatType] = setting;
        }
    }

    public void AddStatPoints(int points)
    {
        AvailablePoints += points;
    }

    public bool TryInvestPoint(StatType statType)
    {
        if (AvailablePoints <= 0) return false;
        if (!settingsMap.TryGetValue(statType, out StatInvestSettings settings)) return false;

        Stat stat = unitStats.GetStat(statType);
        if (stat == null) return false;

        if (stat.GetInvestedPoints() >= settings.MaxPoints) return false;

        stat.InvestPoint(settings.IncreaseAmount);
        AvailablePoints--;
        return true;
    }

    public int GetMaxInvestablePoints(StatType statType)
    {
        return settingsMap.TryGetValue(statType, out StatInvestSettings settings)
            ? settings.MaxPoints
            : 0;
    }
}