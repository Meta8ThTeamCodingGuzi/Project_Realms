using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stat : MonoBehaviour
{
    private float baseValue;
    private readonly List<StatModifier> modifiers;
    private bool isDirty = true;
    private float lastValue;
    
    //�⺻ ���� ������
    public Stat(float baseValue)
    {
        this.baseValue = baseValue;
        modifiers = new List<StatModifier>();
    }

    #region ������Ƽ�� (���� ��� ����)
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

    #region ���� ���� ���� �޼����
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

    #region ��ó�� ���Ȱ�� ���۸޼���
    /// <summary>
    /// Ư�� ��ó(SourceType)���� �� ��� ���� �����ڵ��� ��ȯ�մϴ�.
    /// </summary>
    /// <param name="sourceType">ã���� �ϴ� ��ó Ÿ�� (��: Equipment, Buff, Skill ��)</param>
    /// <returns>�ش� ��ó�� ��� StatModifier ����Ʈ</returns>
    /// <example>
    /// // ��񿡼� ���� ��� ������ ��������
    /// List<StatModifier> equipmentMods = stat.GetModifiersFromSource(SourceType.Equipment);
    /// 
    /// // �������� ���� ��� ������ ��������
    /// List<StatModifier> buffMods = stat.GetModifiersFromSource(SourceType.Buff);
    /// </example>
    public List<StatModifier> GetModifiersFromSource(SourceType sourceType)
    {
        return modifiers.Where(mod => mod.SourceType == sourceType).ToList();
    }
    /// <summary>
    /// Ư�� ��ó(SourceType)�� ���� �����ڰ� �����ϴ��� Ȯ���մϴ�.
    /// </summary>
    /// <param name="sourceType">Ȯ���ϰ��� �ϴ� ��ó Ÿ��</param>
    /// <returns>�ش� ��ó�� �����ڰ� �ϳ��� ������ true, ������ false</returns>
    /// <example>
    /// // ������� �ִ��� Ȯ��
    /// if (stat.HasModifierFromSource(SourceType.Debuff))
    /// {
    ///     Debug.Log("������� �������Դϴ�!");
    /// }
    /// 
    /// // ��� ȿ���� �ִ��� Ȯ��
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

        // 1�ܰ�: �ܼ� ����
        foreach (var mod in sourceModifiers.Where(m => m.Type == StatModifierType.Flat))
        {
            value += mod.Value;
        }

        // 2�ܰ�: �ۼ�Ʈ ����
        foreach (var mod in sourceModifiers.Where(m => m.Type == StatModifierType.PercentAdd))
        {
            sumPercentAdd += mod.Value;
        }
        if (sumPercentAdd != 0)
        {
            value *= (1 + sumPercentAdd);
        }

        // 3�ܰ�: �ۼ�Ʈ ����
        foreach (var mod in sourceModifiers.Where(m => m.Type == StatModifierType.PercentMult))
        {
            value *= (1 + mod.Value);
        }

        return value;
    }


    #endregion
}
