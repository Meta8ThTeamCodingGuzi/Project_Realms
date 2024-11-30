using UnityEngine;

[RequireComponent(typeof(PlayerStat))]
public class StatPointSystem : MonoBehaviour
{
    private PlayerStat playerStat;
    public int AvailablePoints { get; private set; }

    public void Initialize()
    {
        playerStat = GetComponent<PlayerStat>();
    }

    public void AddStatPoints(int points)
    {
        AvailablePoints += points;
    }

    public bool TryInvestPoint(StatType statType)
    {
        if (AvailablePoints <= 0) return false;

        Stat stat = playerStat.GetStat(statType);
        if (stat == null) return false;

        float increaseAmount = playerStat.GetPointIncreaseAmount(statType);
        if (increaseAmount <= 0) return false;  // 투자 불가능한 스탯

        stat.InvestPoint(increaseAmount);
        AvailablePoints--;
        return true;
    }
}