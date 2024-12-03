using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillData data;
    [SerializeField] public SkillStat skillStat;
    [SerializeField] private AnimationClip animaClip;

    private float currentCooldown = 0f;

    public bool IsOnCooldown => currentCooldown > 0f;
    public float RemainingCooldown => currentCooldown;
    public float TotalCooldown => skillStat.GetStatValue<float>(SkillStatType.Cooldown);

    public virtual void Initialize()
    {
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
        return new StatModifier(costmana, StatModifierType.Flat, SourceType.Skill);
    }

    public virtual bool TryUseSkill()
    {
        if(data.skillID != SkillID.BasicSwordAttack) 
        {
            float costmana = -skillStat.GetStatValue<float>(SkillStatType.ManaCost);
            if (IsOnCooldown)
            {
                Debug.Log($"스킬이 쿨다운 중입니다. 남은 시간: {currentCooldown:F1}초");
                return false;
            }

            if (GameManager.Instance.player.CharacterStats.GetStatValue(StatType.Mana) < costmana)
            {
                Debug.Log("마나가 부족합니다");
                return false;
            }

            GameManager.Instance.player.CharacterStats.AddModifier(StatType.Mana, CalcManaCost(costmana));
            if (animaClip != null && !IsCurrentAnimation(animaClip))
            {
                GameManager.Instance.player.PlayerAnimController.Clipchange(animaClip);
            }

            var currentState = GameManager.Instance.player.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentState.normalizedTime >= 0.9f || !currentState.IsName("Attack"))
            {
                GameManager.Instance.player.PlayerAnimator.SetFloat("AttackSpeed",
                    GameManager.Instance.player.CharacterStats.GetStatValue(StatType.AttackSpeed) / 2f);
                GameManager.Instance.player.PlayerAnimator.SetTrigger("Attack");
            }
        }

        StartCoroutine(UseSkillWithDelay());

        // 쿨다운이 0보다 큰 경우에만 쿨다운 시작
        if (TotalCooldown > 0)
        {
            StartCooldown();
        }

        return true;
    }
    private bool IsCurrentAnimation(AnimationClip clip)
    {
        var currentClip = GameManager.Instance.player.PlayerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip;
        return currentClip == clip;
    }

    public virtual IEnumerator UseSkillWithDelay() 
    {
        yield return new WaitForSeconds(0.3f);
        UseSkill();
    }

    protected abstract void UseSkill();

    protected virtual void StartCooldown()
    {
        if (TotalCooldown <= 0)
        {
            Debug.LogWarning($"{gameObject.name} 쿨 없음");
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
            //  ŸԿ  
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
