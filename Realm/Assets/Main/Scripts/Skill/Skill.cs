using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillData data;
    [SerializeField] public SkillStat skillStat;
    [SerializeField] protected AnimationClip animaClip;
    private Unit owner;
    public Unit Owner => owner;
    protected bool isSkillInProgress = false;

    protected float currentCooldown = 0f;

    public bool IsOnCooldown => currentCooldown > 0f;
    public float RemainingCooldown => currentCooldown;
    public float TotalCooldown => skillStat.GetStatValue<float>(SkillStatType.Cooldown);

    public virtual void Initialize(Unit owner)
    {
        this.owner = owner;

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
    protected StatModifier CalcManaCost(float costmana)
    {
        return new StatModifier(costmana, StatModifierType.Flat, SourceType.Skill);
    }

    public virtual bool TryUseSkill()
    {
        if (data.skillID != SkillID.BasicSwordAttack || data.skillID != SkillID.BasicBowAttack)
        {
            float costmana = -skillStat.GetStatValue<float>(SkillStatType.ManaCost);
            if (IsOnCooldown)
            {
                return false;
            }

            if (owner.CharacterStats.GetStatValue(StatType.Mana) < costmana)
            {
                return false;
            }

            owner.CharacterStats.AddModifier(StatType.Mana, CalcManaCost(costmana));
            if(owner is Player) 
            {
                if (animaClip != null)
                {
                    GameManager.Instance.player.AnimController.Clipchange(animaClip);
                }
            }            
            //TODO : 마이크로컨트롤
            owner.Animator.SetFloat("AttackSpeed",
                3f);
        }

        UseSkill();

        if (TotalCooldown > 0)
        {
            StartCooldown();
        }

        return true;
    }

    protected IEnumerator SkillSequenceTimer()
    {
        isSkillInProgress = true;

        float animationSpeed = GameManager.Instance.player.CharacterStats.GetStatValue(StatType.AttackSpeed);
        float actualDuration = animaClip.length / animationSpeed;

        yield return new WaitForSeconds(actualDuration);

        isSkillInProgress = false;
    }

    public bool IsSkillInProgress => isSkillInProgress;

    private bool IsCurrentAnimation(AnimationClip clip)
    {
        var currentClip = GameManager.Instance.player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip;
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
