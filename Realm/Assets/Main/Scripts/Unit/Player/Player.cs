using UnityEngine;

public class Player : Unit
{
    protected override void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        if (characterStats == null)
        {
            UnitStats unitStats = gameObject.AddComponent<UnitStats>();
            characterStats = unitStats;
        }
    }

    public float GetAttack()
    {
        return characterStats.GetStatValue(StatType.Attack);
    }

    public float GetDefense()
    {
        return characterStats.GetStatValue(StatType.Defense);
    }
}
