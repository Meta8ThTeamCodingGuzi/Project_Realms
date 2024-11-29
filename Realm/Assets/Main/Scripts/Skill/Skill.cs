using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillData data;
    [SerializeField] public SkillStat skillStat;

    private float currentCooldown = 0f;

    public bool IsOnCooldown => currentCooldown > 0f;
    public float RemainingCooldown => currentCooldown;
    public float TotalCooldown => skillStat.GetStatValue<float>(SkillStatType.Cooldown);

    public virtual void Initialize()
    {
        print("��ų �ʱ�ȭ �Լ� ȣ��");
        if (skillStat == null)
        {
            skillStat = GetComponent<SkillStat>();
            if (skillStat == null)
            {
                Debug.LogError("SkillStat component not found!");
            }
        }
    }

    private void Update()
    {
        // ��ٿ� ����
        if (currentCooldown > 0)
        {
            currentCooldown = Mathf.Max(0, currentCooldown - Time.deltaTime);
        }
    }

    public virtual void LevelUp()
    {
        int currentLevel = skillStat.GetStatValue<int>(SkillStatType.SkillLevel);
        skillStat.SetSkillLevel(currentLevel + 1);
        PrintAllStats();
    }

    public virtual void SetLevel(int level)
    {
        skillStat.SetSkillLevel(level);
    }
    private StatModifier CalcManaCost(float costmana) 
    {
        return new StatModifier(costmana, StatModifierType.Flat,SourceType.Skill);
    }

    // UseSkill�� protected���� public���� �����ϰ� ��ٿ� üũ �߰�
    public virtual bool TryUseSkill()
    {
        float costmana = -skillStat.GetStatValue<float>(SkillStatType.ManaCost);
        if (IsOnCooldown)
        {
            Debug.Log($"��ų�� ��ٿ� ���Դϴ�. ���� �ð�: {currentCooldown:F1}��");
            return false;
        }
        if (GameManager.Instance.player.CharacterStats.GetStatValue(StatType.Mana)
            < costmana)
        {
            Debug.Log($"���� ������");
            return false;
        }
        else 
        {
            GameManager.Instance.player.CharacterStats.AddModifier(StatType.Mana, CalcManaCost(costmana));
            UseSkill();
        }

        UseSkill();

        // ��ٿ��� 0���� ū ��쿡�� ��ٿ� ����
        if (TotalCooldown > 0)
        {
            StartCooldown();
        }

        return true;
    }

    protected abstract void UseSkill();

    protected virtual void StartCooldown()
    {
        if (TotalCooldown <= 0)
        {
            Debug.LogWarning($"{gameObject.name} ��ų�� ��ٿ��� 0 ���Ϸ� �����Ǿ� �ֽ��ϴ�.");
            return;
        }
        currentCooldown = TotalCooldown;
    }

    protected virtual void PrintAllStats()
    {
        var currentStats = skillStat.GetCurrentStats();
        Debug.Log($"=== {gameObject.name} Level {currentStats[SkillStatType.SkillLevel]} Stats ===");

        foreach (var stat in currentStats)
        {
            // ���� Ÿ�Կ� ���� ������ �������� ���
            string value = FormatStatValue(stat.Key, stat.Value);
            Debug.Log($"{stat.Key}: {value}");
        }
        Debug.Log("=====================================");
    }

    protected virtual string FormatStatValue(SkillStatType type, object value)
    {
        return type switch
        {
            SkillStatType.SkillLevel => $"{(int)value}",
            SkillStatType.Cooldown or
            SkillStatType.Duration or
            SkillStatType.Damage => $"{(float)value:F2}",
            _ => value.ToString()
        };
    }
}
