using UnityEngine;
using System.Collections;

public class SwordSkill : WeaponSkill
{
    private bool isAttacking = false;
    private float baseAttackDuration = 1f;  // �⺻ ���� ���ӽð�

    protected override void UseSkill()
    {
        if (!isAttacking)  // ���� ������ ������ ���� ���ο� ���� ����
        {
            StartCoroutine(SwordAttackRoutine());
        }
    }

    private IEnumerator SwordAttackRoutine()
    {
        isAttacking = true;

        if (weaponCollider != null) weaponCollider.enabled = true;

        float attackDuration = baseAttackDuration / GetPlayerAttackSpeed();
        yield return new WaitForSeconds(attackDuration);

        if (weaponCollider != null) weaponCollider.enabled = false;
        isAttacking = false;
    }

    protected override void OnWeaponHit(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer) != 0)
        {
            if (other.TryGetComponent<Monster>(out Monster monster))
            {
                float totalDamage = GetPlayerDamage();
                monster.TakeDamage(totalDamage);
            }
        }
    }
}