
using UnityEngine;

public class IntStat : Stat
{
    protected int baseValue;
    protected int lastValue;
    protected int pointIncrease; 

    public IntStat(int baseValue, int pointIncrease = 1) : base()
    {
        this.baseValue = baseValue;
        this.pointIncrease = pointIncrease;
    }

    public override object Value
    {
        get
        {
            if (isDirty)
            {
                CalculateFinalValue();
                isDirty = false;
            }
            return lastValue;
        }
    }

    public override void InvestPoint(float increaseAmount)
    {
        investedPoints++;       
        baseValue += Mathf.RoundToInt(increaseAmount);
        isDirty = true;
    }

    protected override void CalculateFinalValue()
    {
        float finalValue = baseValue;
        float sumPercentAdd = 0;

        for (int i = 0; i < modifiers.Count; i++)
        {
            StatModifier mod = modifiers[i];

            if (mod.Type == StatModifierType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == StatModifierType.PercentAdd)
            {
                sumPercentAdd += mod.Value;

                if (i + 1 >= modifiers.Count || modifiers[i + 1].Type != StatModifierType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.Type == StatModifierType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
        }

        lastValue = Mathf.RoundToInt(finalValue);
    }
}