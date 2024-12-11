using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Pet : Unit
{
    private Player targetPlayer;
    public Transform enemyTarget;
    public Transform attackPoint;
    private new NavMeshAgent agent;
    public Animator animator;
    [Header("������� �Ÿ��α�"), Range(1f, 12f)]
    public float playDis;
    [Header("������� �Ÿ��α�"), Range(1f, 12f)]
    public float EnemyDis;
    [Header("���� ȣ��Ÿ�"), Range(15f, 25f)]
    public float playerTellme = 15f;

    [SerializeField] private Transform FirePoint;

    public GameObject tellEffect;

    public Skill petSkill;

    private float lastAttackTime = 0f;  // 마지막 공격 시간 추가

    public void Initialize(Player player)
    {
        base.Initialize();
        targetPlayer = player;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Animator = animator;
        agent.stoppingDistance = 8;
        petSkill = Instantiate(petSkill, transform);
        petSkill.Initialize(this);
    }

    private void Update()
    {
        Move();
        SetAnim();
        PlayerTell();
    }

    private void Move()
    {
        if (enemyTarget != null)
        {
            agent.stoppingDistance = EnemyDis;
            agent.SetDestination(enemyTarget.position);
            transform.LookAt(enemyTarget.position);
        }
        else if (targetPlayer.transform != null)
        {
            agent.stoppingDistance = playDis;
            agent.SetDestination(targetPlayer.transform.position);
        }
    }

    private void PlayerTell()
    {
        if (Vector3.Distance(transform.position, targetPlayer.transform.position) >= playerTellme)
        {
            transform.position = targetPlayer.transform.position;
            enemyTarget = null;
            animator.SetTrigger("Tell");
            StartCoroutine(SpawnEffect());
        }

    }

    private IEnumerator SpawnEffect()
    {
        GameObject petEffect = Instantiate(tellEffect, this.transform);
        yield return new WaitForSeconds(1.5f);
        Destroy(petEffect);
    }

    private void SetAnim()
    {
        if (agent.velocity.sqrMagnitude < 0.01f)
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }
    }

    private void FireSkill()
    {
        if (enemyTarget == null) return;

        // 공격 속도에 따른 딜레이 체크
        float attackDelay = 1f / CharacterStats.GetStatValue(StatType.AttackSpeed);
        if (Time.time - lastAttackTime < attackDelay) return;

        // 적 방향으로 회전
        Vector3 directionToTarget = (enemyTarget.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToTarget);

        if (petSkill.TryUseSkill())
        {
            lastAttackTime = Time.time;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster))
        {
            enemyTarget = other.transform;
            FireSkill();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster))
        {
            enemyTarget = null;  // �� ���� ����
        }
    }
}
