using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeArea : MonoBehaviour
{
    private AreaData areaData;
    private float durationTime;
    private Coroutine attackCoroutine;
  

    public void Initialize(AreaData Data)
    {
        this.areaData = Data;
        transform.localScale = Vector3.one * areaData.areaScale;
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
            if (areaData.duration < durationTime) PoolManager.Instance.Despawn<FakeArea>(this);
            Collider[] colliders = Physics.OverlapSphere(transform.position,areaData.areaScale);

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<Monster>(out Monster monster))
                {
                    print("몬스터 TakeDamage호출");
                    monster.TakeDamage(areaData.damage);
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
            PoolManager.Instance.Despawn<FakeArea>(this);
        }
        
    }

    private void OnDestroy()
    {
        StopAttack();
    }
}
