using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : Unit, IPoolable
{
    private MonsterStateHandler m_StateHandler;
    public MonsterStateHandler M_StateHandler => m_StateHandler;

    public Player targetPlayer;

    [SerializeField]
    private Transform[] setPatrolTransform;


    private int patrolKey = 0;
    public Transform nowTarget;


    protected override void Initialize()
    {
        m_StateHandler = GetComponent<MonsterStateHandler>();
        base.Initialize();
    }
    public bool ReachNowPoint()
    {
        float distanceToTarget = Vector3.Distance(transform.position, nowTarget.position);

        return distanceToTarget <= agent.stoppingDistance ||
       !agent.hasPath ||
       agent.pathStatus == NavMeshPathStatus.PathInvalid ||
       agent.velocity.sqrMagnitude < 0.1f;
    }
    public void targetMove(Unit unit)
    {
        if (unit != null && agent.isActiveAndEnabled && IsAlive)
        {
            agent.SetDestination(unit.transform.position);
        }


    }

    private void Update()
    {
        m_StateHandler.HandleUpdate();
    }

    public bool FindPlayer(float Detection)
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, Detection);

        foreach (Collider collider in colliders) 
        {
            if (collider.TryGetComponent<Player>(out Player player))
            {
                targetPlayer = player;
                return true;
            }

        }
        return false;
    }

    protected override IEnumerator AttackRoutine(Unit target)
    {
        while (IsAlive && target != null && target.IsAlive)
        {
            if (CanAttack(target))
            {
                agent.ResetPath();
                float currentTime = Time.time;
                float attackSpeed = characterStats.GetStatValue(StatType.AttackSpeed);
                float timeBetweenAttacks = 1f / attackSpeed;

                if (currentTime - lastAttackTime >= timeBetweenAttacks)
                {
                    PerformAttack(target);
                    lastAttackTime = currentTime;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }

        attackCoroutine = null;
    }
    public void nextPatrol()
    {
        patrolKey++;
        if (patrolKey > setPatrolTransform.Length)
        { 
            patrolKey = 0;
            nowTarget = setPatrolTransform[patrolKey];
            return;
        }
        nowTarget = setPatrolTransform[patrolKey];
    }
    public void OnReturnToPool()
    {
        throw new System.NotImplementedException();
    }

    public void OnSpawnFromPool()
    {
        throw new System.NotImplementedException();
    }

}
