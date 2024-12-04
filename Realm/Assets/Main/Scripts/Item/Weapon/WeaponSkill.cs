using UnityEngine;
using System.Collections;

public abstract class WeaponSkill : Skill
{
    protected WeaponHolder weaponHolder;
    protected Player player;
    protected ICharacterStats playerStats;
    protected bool isAttackInProgress = false;

    [Header("Weapon Settings")]
    [SerializeField] protected float attackAngle = 90f;
    [SerializeField] protected LayerMask targetLayer;

    public override void Initialize()
    {
        player = GetComponentInParent<Player>();
        if (player != null)
        {
            playerStats = player.GetComponent<ICharacterStats>();
            weaponHolder = player.GetComponent<WeaponHolder>();
        }
    }

    public override bool TryUseSkill()
    {
        Debug.Log($"[WeaponSkill] TryUseSkill called");
        if (!CanUseSkill()) return false;
        if (isAttackInProgress) return false;

        isAttackInProgress = true;
        UseSkill();
        return true;
    }

    protected virtual bool CanUseSkill()
    {
        Debug.Log($"[WeaponSkill] CanUseSkill check");
        if (player == null || !player.IsAlive) return false;

        if (data.skillID == SkillID.BasicSwordAttack || data.skillID == SkillID.BasicBowAttack)
        {
            return player.TargetMonster != null;
        }

        return true;
    }

    protected virtual float GetAttackDelay()
    {
        if (data.skillID == SkillID.BasicSwordAttack || data.skillID == SkillID.BasicBowAttack)
        {
            return 1f / GetPlayerAttackSpeed();
        }
        return 0.3f;
    }

    protected float GetPlayerDamage()
    {
        return playerStats?.GetStatValue(StatType.Damage) ?? 0f;
    }

    protected float GetPlayerAttackSpeed()
    {
        return playerStats?.GetStatValue(StatType.AttackSpeed) ?? 1f;
    }

    protected float GetAttackRange()
    {
        return playerStats?.GetStatValue(StatType.AttackRange) ?? 2f;
    }

    protected void OnAttackComplete()
    {
        isAttackInProgress = false;
    }
}