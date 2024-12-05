using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectileSkill : ProjectileSkill
{
    private Monster monster;
    private Player targetplayer;

    public override void Start()
    {
        print("필요없음");
    }

    protected override void ValidateComponents()
    {
        print("필요없음");
    }

    public override void Initialize()
    {
        base.Initialize();
        monster = GetComponent<Monster>();
    }

    public override bool TryUseSkill()
    {
        if (data.skillID != SkillID.BasicSwordAttack)
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
            if (animaClip != null)
            {
                GameManager.Instance.player.PlayerAnimController.Clipchange(animaClip);
                StartCoroutine(SkillSequenceTimer());
            }
        }


        UseSkill();

        if (TotalCooldown > 0)
        {
            StartCooldown();
        }

        return true;
    }

    private void FindPlayer()
    {
        if(monster.targetPlayer != null)
        {
            targetplayer = monster.targetPlayer;
        }
    }

    protected override void FireProjectile(ProjectileData data)
    {
        FindPlayer();
        transform.LookAt(targetplayer.transform.position);
        base.FireProjectile(data);
    }
}
