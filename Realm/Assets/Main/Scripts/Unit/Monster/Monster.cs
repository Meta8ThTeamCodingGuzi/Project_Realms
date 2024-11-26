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
    private Transform[] setPatrolTransforms;

    private List<Vector3> patrolPoint = new List<Vector3>();


    private int patrolKey = 0;
    public Vector3 nowTarget;

    //TODO:: 몬스터 스폰로직 생기면 지우세요
    private void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    { 
        foreach (Transform setPatrolTransform in setPatrolTransforms)
        {
            patrolPoint.Add(setPatrolTransform.position);
        }

        if (m_StateHandler == null)
        {
            m_StateHandler = new MonsterStateHandler(this);
        }
        m_StateHandler.Initialize();
        base.Initialize();
        targetPlayer = null;

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
        if (patrolKey >= patrolPoint.Count)
        { 
            patrolKey = 0;
            nowTarget = patrolPoint[patrolKey];
            return;
        }
        nowTarget = patrolPoint[patrolKey];
    }
    public void OnReturnToPool()
    {
 
    }

    public void OnSpawnFromPool()
    {
        float playerLevel = GameManager.Instance.player.CharacterStats.GetStatValue(StatType.Level);
        characterStats.AddModifier(StatType.Level, new StatModifier(playerLevel, StatModifierType.Flat,this,SourceType.BaseStats));  
        Initialize();
    }

}
