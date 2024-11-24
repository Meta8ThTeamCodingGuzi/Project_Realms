using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stat : MonoBehaviour
{
    private float baseValue;
    private readonly List<StatModifier> modifiers;
    private bool isDirty = true;
    private float lastValue;
    
    //기본 스탯 생성자
    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        modifiers = new List<StatModifier>();
    }

    #region 프로퍼티들 (스탯 계산 관련)
    public float BaseValue
    {
        get => baseValue;
        set
        {
            baseValue = value;
            isDirty = true;
        }
    }
   
    public float Value
    {
        get
        {
            if (isDirty)
            {
                lastValue = CalculateFinalValue();
                isDirty = false;
            }
            return lastValue;
        }
    }
    #endregion

    #region 스탯 변경 관련 메서드들
    public void AddModifier(StatModifier modifier)
    {
        modifiers.Add(modifier);
        isDirty = true;
    }

    public void RemoveModifier(StatModifier modifier)
    {
        modifiers.Remove(modifier);
        isDirty = true;
    }

    private float CalculateFinalValue()
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

        return finalValue;
    }
    #endregion

    #region 출처별 스탯계산 헬퍼메서드
    /// <summary>
    /// 특정 출처(SourceType)에서 온 모든 스탯 수정자들을 반환합니다.
    /// </summary>
    /// <param name="sourceType">찾고자 하는 출처 타입 (예: Equipment, Buff, Skill 등)</param>
    /// <returns>해당 출처의 모든 StatModifier 리스트</returns>
    /// <example>
    /// // 장비에서 오는 모든 수정자 가져오기
    /// List<StatModifier> equipmentMods = stat.GetModifiersFromSource(SourceType.Equipment);
    /// 
    /// // 버프에서 오는 모든 수정자 가져오기
    /// List<StatModifier> buffMods = stat.GetModifiersFromSource(SourceType.Buff);
    /// </example>
    public List<StatModifier> GetModifiersFromSource(SourceType sourceType)
    {
        return modifiers.Where(mod => mod.SourceType == sourceType).ToList();
    }
    /// <summary>
    /// 특정 출처(SourceType)의 스탯 수정자가 존재하는지 확인합니다.
    /// </summary>
    /// <param name="sourceType">확인하고자 하는 출처 타입</param>
    /// <returns>해당 출처의 수정자가 하나라도 있으면 true, 없으면 false</returns>
    /// <example>
    /// // 디버프가 있는지 확인
    /// if (stat.HasModifierFromSource(SourceType.Debuff))
    /// {
    ///     Debug.Log("디버프가 적용중입니다!");
    /// }
    /// 
    /// // 장비 효과가 있는지 확인
    /// bool hasEquipment = stat.HasModifierFromSource(SourceType.Equipment);
    /// </example>
    public bool HasModifierFromSource(SourceType sourceType)
    {
        return modifiers.Any(mod => mod.SourceType == sourceType);
    }

    public void RemoveAllModifiersFromSource(SourceType sourceType)
    {
        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            if (modifiers[i].SourceType == sourceType)
            {
                modifiers.RemoveAt(i);
                isDirty = true;
            }
        }
    }
    public bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            if (modifiers[i].Source == source)
            {
                modifiers.RemoveAt(i);
                isDirty = true;
                didRemove = true;
            }
        }
        return didRemove;
    }
    public float GetTotalValueFromSource(SourceType sourceType)
    {
        float value = 0f;
        float sumPercentAdd = 0;
        var sourceModifiers = GetModifiersFromSource(sourceType);

        // 1단계: 단순 덧셈
        foreach (var mod in sourceModifiers.Where(m => m.Type == StatModifierType.Flat))
        {
            value += mod.Value;
        }

        // 2단계: 퍼센트 덧셈
        foreach (var mod in sourceModifiers.Where(m => m.Type == StatModifierType.PercentAdd))
        {
            sumPercentAdd += mod.Value;
        }
        if (sumPercentAdd != 0)
        {
            value *= (1 + sumPercentAdd);
        }

        // 3단계: 퍼센트 곱셈
        foreach (var mod in sourceModifiers.Where(m => m.Type == StatModifierType.PercentMult))
        {
            value *= (1 + mod.Value);
        }

        return value;
    }


    #endregion
}
