using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SwordSkill : WeaponSkill
{

    protected override void UseSkill()
    {
        if (isAttacking) return;

        Monster targetMonster = player.TargetMonster;
        if (targetMonster != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetMonster.transform.position);
            float attackRange = GetAttackRange();

            if (distanceToTarget <= attackRange)
            {
                player.StopMoving();
                player.PlayerAnimator.SetTrigger("Attack");
                player.transform.LookAt(targetMonster.transform);
                StartCoroutine(SwordAttackRoutine());
            }
            else
            {
                player.MoveTo(targetMonster.transform.position);
            }
        }
    }

    private IEnumerator SwordAttackRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(0.2f);

        PerformSectorAttack();

        yield return new WaitForSeconds(0.1f);

        isAttacking = false;
    }

    private void PerformSectorAttack()
    {
        Vector3 playerPosition = transform.position;
        Vector3 forward = player.transform.forward;
        float attackRange = GetAttackRange();

        Collider[] hitColliders = Physics.OverlapSphere(playerPosition, attackRange, targetLayer);

        foreach (Collider collider in hitColliders)
        {
            Vector3 directionToTarget = (collider.transform.position - playerPosition).normalized;

            float angle = Vector3.Angle(forward, directionToTarget);

            if (angle <= attackAngle / 2)
            {
                if (collider.TryGetComponent<Monster>(out Monster monster))
                {
                    float totalDamage = GetPlayerDamage();
                    monster.TakeDamage(totalDamage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (player == null) return;

        Vector3 position = transform.position;
        Vector3 forward = transform.forward;
        float range = GetAttackRange();

        Gizmos.color = Color.red;

        int segments = 20;
        float angleStep = attackAngle / segments;
        Vector3 previousPoint = position + Quaternion.Euler(0, -attackAngle / 2, 0) * forward * range;

        for (int i = 0; i <= segments; i++)
        {
            float currentAngle = (-attackAngle / 2) + (angleStep * i);
            Vector3 currentPoint = position + Quaternion.Euler(0, currentAngle, 0) * forward * range;

            Gizmos.DrawLine(position, currentPoint);
            if (i > 0)
                Gizmos.DrawLine(previousPoint, currentPoint);

            previousPoint = currentPoint;
        }
    }
}