using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect : MonoBehaviour
{
    private AreaEffectData areaData;
    private float durationTime;
    private Coroutine attackCoroutine;  

    public void Initialize(AreaEffectData Data)
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

    private void Update()
    {
        durationTime += Time.deltaTime;

        if (durationTime >= areaData.duration) 
        {
            StopCoroutine(attackCoroutine);
            PoolManager.Instance.Despawn<AreaEffect>(this);
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (areaData.duration < durationTime) PoolManager.Instance.Despawn<AreaEffect>(this);
            Collider[] colliders = Physics.OverlapSphere(transform.position,areaData.areaScale);

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<Monster>(out Monster monster))
                {
                    monster.TakeDamage(areaData.damage);
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
