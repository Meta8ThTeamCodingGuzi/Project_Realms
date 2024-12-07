using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAreaEffect : AreaEffect
{
    [SerializeField] private float AreaDelay = 0f;

    private void Update()
    {
        durationTime += Time.deltaTime;

        if (durationTime >= areaData.duration + AreaDelay)
        {
            StopCoroutine(attackCoroutine);
            PoolManager.Instance.Despawn<AreaEffect>(this);  
        }   
    }

    public override IEnumerator AttackRoutine()
    {
        bool isOwnerPlayer = areaData.owner is Player;

        yield return new WaitForSeconds(AreaDelay);

        BoomAttack(isOwnerPlayer);
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (areaData.duration + AreaDelay <= durationTime)
            {
                PoolManager.Instance.Despawn<AreaEffect>(this);
                yield break;
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, areaData.areaScale);
            foreach (Collider collider in colliders)
            {
                if (!collider.TryGetComponent<Unit>(out Unit targetUnit))
                    continue;

                if ((isOwnerPlayer && targetUnit is Monster) || (!isOwnerPlayer && targetUnit is Player))
                {
                    targetUnit.TakeDamage(areaData.damage * 0.5f);
                }
            }
        }
    }

    private void BoomAttack(bool isOwnerPlayer)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, areaData.areaScale * 1.2f);
        foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent<Unit>(out Unit targetUnit))
                continue;

            if ((isOwnerPlayer && targetUnit is Monster) || (!isOwnerPlayer && targetUnit is Player))
            {
                targetUnit.TakeDamage(areaData.damage);
            }
        }
    }


}
