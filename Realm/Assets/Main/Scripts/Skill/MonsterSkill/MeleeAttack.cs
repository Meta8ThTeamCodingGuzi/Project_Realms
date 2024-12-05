using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class MeleeAttack : Skill
{

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float attackOffset;

    private bool isAttacking = false;
    private Monster monster;
    ICharacterStats monsterStats;

    private float monsterBoxScale;


    public override void Initialize(Unit owner)
    {
        base.Initialize(owner);
        monster = GetComponent<Monster>();
        if (monster != null)
        {
            monsterStats = monster.GetComponent<ICharacterStats>();
        }
        monsterBoxScale = monster.transform.localScale.x/2f;
        attackOffset = GetAttackRange() / 2f;
    }

    public override bool TryUseSkill()
    {
        if (isAttacking) return false;

        UseSkill();
        return true;
    }

    protected override void UseSkill()
    {


        Player targetPlayer = monster.targetPlayer;
        if (targetPlayer != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPlayer.transform.position);
            float attackRange = GetAttackRange();


            if (distanceToTarget <= attackRange)
            {
                monster.StopMoving();
                monster.M_Animator.SetTrigger("Attack");
                monster.transform.LookAt(targetPlayer.transform);            
                StartCoroutine(MonsterAttackRoutine());
            }
        }


    }


    private IEnumerator MonsterAttackRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        PerformSectorAttack();

        yield return new WaitForSeconds(GetAttackSpeed());

        isAttacking = false;
    }

    private void PerformSectorAttack()
    {
        Vector3 monsterPosition = transform.position;
        Vector3 forward = monster.transform.forward;
        float attackRange = GetAttackRange();

        Vector3 attackCenter = monsterPosition + (forward * attackOffset);

        Collider[] hitColliders = Physics.OverlapBox(attackCenter,new Vector3(monsterBoxScale,1f,1*attackRange/2f), transform.rotation, targetLayer);

        foreach (Collider collider in hitColliders)
        {
            Vector3 directionToTarget = (collider.transform.position - monsterPosition).normalized;

            if (collider.TryGetComponent<Player>(out Player player))
            {
                float totalDamage = GetMonsterDamage();
                player.TakeDamage(totalDamage); 
                print("플레이어 Take데미지 호출성공");
            }


        }

    }
    private void OnDrawGizmos()
    {
        if (monster == null) return;

        Gizmos.color = Color.red;
        Vector3 attackCenter = transform.position + (transform.forward * attackOffset);

        Gizmos.matrix = Matrix4x4.TRS(attackCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(monsterBoxScale*2f,2f,1*GetAttackRange()));
    }



    protected float GetMonsterDamage()
    {
        return monsterStats?.GetStatValue(StatType.Damage) ?? 0f;
    }

    protected float GetAttackSpeed()
    {
        return 1f/monsterStats?.GetStatValue(StatType.AttackSpeed) ?? 1f;
    }

    protected float GetAttackRange()
    {
        return monsterStats?.GetStatValue(StatType.AttackRange) ?? 2f;
    }


}