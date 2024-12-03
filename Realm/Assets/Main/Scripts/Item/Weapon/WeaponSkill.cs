using UnityEngine;
using System.Collections;

public abstract class WeaponSkill : Skill
{
    protected WeaponHolder weaponHolder;
    protected Collider weaponCollider;
    protected Player player;
    protected ICharacterStats playerStats;

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

            if (weaponHolder != null)
            {
                var collider = weaponHolder.GetWeaponComponents();
                weaponCollider = collider;
            }
        }
    }

    // 무기가 교체될 때 호출
    public virtual void UpdateWeaponComponents()
    {
        if (weaponHolder != null)
        {
            var collider = weaponHolder.GetWeaponComponents();
            weaponCollider = collider;
        }
    }

    public override bool TryUseSkill()
    {
        if (data.skillID == SkillID.BasicSwordAttack || data.skillID == SkillID.BasicBowAttack)
        {
            StartCoroutine(UseSkillWithDelay());
            return true;
        }

        return base.TryUseSkill();
    }

    public override IEnumerator UseSkillWithDelay()
    {
        if (data.skillID == SkillID.BasicSwordAttack || data.skillID == SkillID.BasicBowAttack)
        {
            yield return new WaitForSeconds(0.3f / GetPlayerAttackSpeed());
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
        }
        UseSkill();
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

    protected abstract void OnWeaponHit(Collider other);
}