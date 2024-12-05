using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectileSkill : ProjectileSkill
{
    private Monster monster;
    private Player targetplayer;


    public override void Initialize(Unit owner)
    {
        base.Initialize(owner);
        monster = GetComponent<Monster>();
    }

    public override bool TryUseSkill()
    {
        if (data.skillID != SkillID.BasicSwordAttack)
        {
            float costmana = -skillStat.GetStatValue<float>(SkillStatType.ManaCost);
            if (IsOnCooldown)
            {
                Debug.Log($"��ų�� ��ٿ� ���Դϴ�. ���� �ð�: {currentCooldown:F1}��");
                return false;
            }

            if (GameManager.Instance.player.CharacterStats.GetStatValue(StatType.Mana) < costmana)
            {
                Debug.Log("������ �����մϴ�");
                return false;
            }

            GameManager.Instance.player.CharacterStats.AddModifier(StatType.Mana, CalcManaCost(costmana));
            if (animaClip != null)
            {
                GameManager.Instance.player.AnimController.Clipchange(animaClip);
                //tartCoroutine(SkillSequenceTimer());
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
