using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PointAttack : MonoBehaviour
{
    private PointData PointAttackData;
    private Coroutine attackCoroutine;


    public void Initialize(PointData Data)
    {
        this.PointAttackData = Data;
        transform.localScale = Vector3.one * PointAttackData.Scale;
        AttackRoutine();
    }

    private IEnumerator AttackRoutine()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, PointAttackData.Scale);
        yield return new WaitForSeconds(0.5f);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Monster>(out Monster monster))
            {
                print("몬스터 TakeDamage호출");
                monster.TakeDamage(PointAttackData.damage);
                yield return new WaitForSeconds(1.2f);
                PoolManager.Instance.Despawn<PointAttack>(this);
                yield break;
            }
        }
        yield return new WaitForSeconds(1.2f);
        PoolManager.Instance.Despawn<PointAttack>(this);
        yield break;
    }






}
