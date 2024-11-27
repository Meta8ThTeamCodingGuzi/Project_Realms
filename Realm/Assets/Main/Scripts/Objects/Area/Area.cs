using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private AreaData areaData;
    private float durationTime;
    private Coroutine attackCoroutine;
    private ParticleSystem ParticleSystem;
    private Collision Collider;


    public void Initialize(AreaData Data)
    {
        this.areaData = Data;
        transform.localScale = Vector3.one * Data.AreaScale;
        durationTime = 0;
        StartAttack();
    }
    
    private void StartAttack()
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (areaData.Duration < durationTime) PoolManager.Instance.Despawn<Area>(this);
            Collider[] colliders = Physics.OverlapSphere(transform.position, areaData.AreaScale);

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<Monster>(out Monster monster))
                {
                    monster.TakeDamage(areaData.Damage);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    private void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }


    private void OnDestroy()
    {
        StopAttack();
    }
}
