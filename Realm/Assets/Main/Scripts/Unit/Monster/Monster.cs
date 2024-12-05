using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class Monster : Unit
{
    private MonsterStateHandler m_StateHandler;

    [SerializeField]
    private Transform[] setPatrolTransforms;

    private List<Vector3> patrolPoint = new List<Vector3>();

    public bool isattacked { set; get; } = true;
    [SerializeField] private ExpParticle expParticle;

    private int patrolKey = 0;
    public Vector3 currentPatrolPoint { get; set ; }

    public MonsterStateHandler M_StateHandler => m_StateHandler;

    private MonsterStat MonsterStat;

    public MonsterStat monsterStat { get => MonsterStat; set => MonsterStat = value; }

    public ParticleSystem monsterDieParticle;

    [SerializeField] private MonsterType monsterType = MonsterType.Normal;
    public MonsterType MonsterType => monsterType;

    private Skill monsterSkill;
    public Skill Monsterskill { get => monsterSkill; set => monsterSkill = value; }

    protected override void Initialize()
    {
        base.Initialize();
        foreach (Transform setPatrolTransform in setPatrolTransforms)
        {
            patrolPoint.Add(setPatrolTransform.position);
        }

        if (m_StateHandler == null)
        {
            m_StateHandler = new MonsterStateHandler(this);
        }
        m_StateHandler.Initialize();

        Animator = GetComponentInChildren<Animator>();

        if (characterStats != null)
        {
            monsterStat = (MonsterStat)characterStats;
        }

        // 몬스터 타입에 따른 크기 조정
        float sizeMultiplier = monsterType switch
        {
            MonsterType.Elite => 1.2f,
            MonsterType.MiniBoss => 1.5f,
            MonsterType.Boss => 2f,
            MonsterType.Unique => 1.8f,
            _ => 1f // Normal
        };

        transform.localScale *= sizeMultiplier;

        monsterSkill = GetComponent<Skill>();

        monsterSkill.Initialize(this);

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
                if (player.IsAlive)
                {
                    this.Target = player;
                    return true;
                }
            }
        }
        this.Target = null;
        return false;
    }

    public void nextPatrol()
    {
        patrolKey++;
        if (patrolKey >= patrolPoint.Count)
        {
            patrolKey = 0;
            currentPatrolPoint = patrolPoint[patrolKey];
            return;
        }
        currentPatrolPoint = patrolPoint[patrolKey];
    }
    public void MonsterDie()
    {
        StartCoroutine(DieRoutine());
    }

    public IEnumerator DieRoutine()
    {
        Animator.SetTrigger("Die");

        while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            yield return null;
        }

        while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        DropExpParticle();
        ItemManager.Instance.GenerateRandomItem(monsterType, transform.position);
        ParticleSystem mdp = PoolManager.Instance.Spawn<ParticleSystem>(monsterDieParticle.gameObject, transform.position, Quaternion.identity);
        mdp.Play();
        PoolManager.Instance.Despawn(mdp, 1f);
        PoolManager.Instance.Despawn(this);
    }

    private void DropExpParticle()
    {
        float baseExpDrop = characterStats.GetStatValue(StatType.DropExp);

        // 몬스터 타입에 따른 경험치 보정
        float expMultiplier = monsterType switch
        {
            MonsterType.Elite => 2f,
            MonsterType.MiniBoss => 3f,
            MonsterType.Boss => 5f,
            MonsterType.Unique => 4f,
            _ => 1f // Normal
        };

        float totalExpDrop = baseExpDrop * expMultiplier;

        int particleCount = Mathf.Max(1, Mathf.RoundToInt(totalExpDrop / 10f));
        float expPerParticle = totalExpDrop / particleCount;

        for (int i = 0; i < particleCount; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * 1f;
            Vector3 randomPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            ExpParticle particle = PoolManager.Instance.Spawn<ExpParticle>(expParticle.gameObject, randomPosition, Quaternion.identity);
            particle.SetExpAmount(expPerParticle);
        }
    }
    private void OnEnable()
    {
        Initialize(); 
        float playerLevel = GameManager.Instance.player.CharacterStats.GetStatValue(StatType.Level);

        // 몬스터 타입에 따른 레벨 보정
        float levelMultiplier = monsterType switch
        {
            MonsterType.Elite => 1.5f,
            MonsterType.MiniBoss => 2f,
            MonsterType.Boss => 3f,
            MonsterType.Unique => 2.5f,
            _ => 1f // Normal
        };

        int adjustedLevel = Mathf.RoundToInt(playerLevel * levelMultiplier);
        monsterStat.SetMonsterLevel(adjustedLevel);
        this.monsterSkill.SetLevel(adjustedLevel);
    }
    private void OnDisable()
    {
        patrolPoint.Clear();
    }



}
