using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    public Player targetPlayer;  // �÷��̾ ���󰡴� ��ǥ
    private Transform enemyTarget;  // �� ��ǥ
    public string targetTag = "Enemy";  // �� �±� ����
    public GameObject pouBallPrefab;  // ����ü ������
    public Transform attackPoint;  // ����ü �߻� ��ġ
    private bool canAttack = true;
    private NavMeshAgent agent;
    private Animator animator;
    [Header("������� �Ÿ��α�"), Range(1f, 12f)]
    public float playDis;
    [Header("������� �Ÿ��α�"), Range(1f, 12f)]
    public float EnemyDis;
    [Header("���� ȣ��Ÿ�"), Range(15f, 25f)]
    public float playerTellme = 15f;

    [SerializeField] private Transform FirePoint;

    public GameObject tellEffect;

    public ProjectileSkill pouBall;

    private ProjectileSkill poupouball;


    // ������ ��ų �߻� �ð�

    //void Start()
    //{
    //    agent = GetComponent<NavMeshAgent>();
    //    animator = GetComponent<Animator>();
    //    //playerTarget = GameManager.Instance.player.transform;
    //    agent.stoppingDistance = 8;  // �÷��̾���� ���� �Ÿ�
    //}

    public void Initialize(Player player)
    {
        targetPlayer = player;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //playerTarget = GameManager.Instance.player.transform;
        agent.stoppingDistance = 8;  // �÷��̾���� ���� �Ÿ�
        poupouball = Instantiate(pouBall, transform);
        poupouball.Initialize(player);
        poupouball.firePoint = FirePoint;
    }

    void Update()
    {
        Move();
        SetAnim();
        PlayerTell();
    }

    // �̵� ����: ���� ������ �� ����, ������ �÷��̾� ����
    private void Move()
    {
        if (enemyTarget != null)  // ���� �����Ǿ�����
        {
            agent.stoppingDistance = EnemyDis;
            agent.SetDestination(enemyTarget.position);
            transform.LookAt(enemyTarget.position);
        }
        else if (targetPlayer.transform != null)  // ���� ������ �÷��̾� ����
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

    // �ִϸ��̼� ����: �����ӿ� ���� �ִϸ��̼� ����
    private void SetAnim()
    {
        if (agent.velocity.sqrMagnitude < 0.01f)  // �̵����� ���� ��
        {
            animator.SetBool("isRunning", false);  // IDLE �ִϸ��̼�
        }
        else  // �̵� ���� ��
        {
            animator.SetBool("isRunning", true);  // RUNNING �ִϸ��̼�
        }
    }

    // ��ų �߻� ����
    private void FireSkill()
    {
        if (attackPoint != null)
        {
            //    // ����ü ���� �� ��ǥ ����
            //    GameObject pouBall = Instantiate(pouBallPrefab, attackPoint.position, Quaternion.identity);
            //    pouBall.GetComponent<PouBall>().SetTarget(enemyTarget.position);  // ��ǥ ����
            //    Debug.Log("�����߽�~!");
            poupouball.TryUseSkill();
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster))//(targetTag) && canAttack)  // ���� ������ ���� �߻�
        {

            enemyTarget = other.transform;  // ���� ���� ������� ����
            FireSkill();  // ���� �߻�
            //canAttack = false;  // ���� �Ұ���

            // ��Ÿ�� ���
            //StartCoroutine(SkillCooldown());  // ��Ÿ�� ���
        }
    }

    // ��Ÿ�� ��� �ڷ�ƾ
    private IEnumerator SkillCooldown()
    {
        yield return new WaitForSeconds(1);
        canAttack = true;  // ��Ÿ�� ���� �� ���� ����
        Debug.Log("��Ÿ�� ��, �ٽ� ���� ����!");
    }


    // ���� ������ ����� ��
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster))
        {
            enemyTarget = null;  // �� ���� ����
        }
    }

    //public IEnumerator AttackDelay(Enemy enemy)
    //{
    //    canAttack = false;

    //    if (animator != null)
    //    {
    //        animator.SetTrigger("Shoot"); // "Attack" Ʈ���� �ߵ�
    //    }


    //    yield return new WaitForSeconds(1);
    //    canAttack = true;
    //}
}
