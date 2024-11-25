using UnityEngine;

public class LevelableStat : FloatStat
{
    protected float growthValue;  // 레벨당 증가량
    protected float growthRate;   // 레벨당 증가율 (퍼센트)
    protected int currentLevel = 1;

    public LevelableStat(float baseValue, float growthValue = 0f, float growthRate = 0f)
        : base(baseValue)
    {
        this.growthValue = growthValue;
        this.growthRate = growthRate;
    }

    public void SetLevel(int level)
    {
        if (level < 1) level = 1;
        currentLevel = level;
        isDirty = true;
    }

    protected override void CalculateFinalValue()
    {
        // 레벨에 따른 기본값 계산
        float leveledBase = baseValue;

        // 고정 증가량
        leveledBase += growthValue * (currentLevel - 1);

        // 비율 증가량
        if (growthRate > 0)
        {
            leveledBase *= Mathf.Pow(1 + growthRate, currentLevel - 1);
        }

        // 이후 기존 modifier 계산
        float finalValue = leveledBase;
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

        lastValue = finalValue;
    }
}