using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    public Transform playerTarget;  // �÷��̾ ���󰡴� ��ǥ
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

    // ������ ��ų �߻� �ð�

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //playerTarget = GameManager.Instance.player.transform;
        agent.stoppingDistance = 8;  // �÷��̾���� ���� �Ÿ�
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
        else if (playerTarget != null)  // ���� ������ �÷��̾� ����
        {
            agent.stoppingDistance = playDis;
            agent.SetDestination(playerTarget.position);
        }
    }




    private void PlayerTell()
    {
        if (Vector3.Distance(transform.position, playerTarget.position) >= playerTellme)
        {
            transform.position = playerTarget.position;
            enemyTarget = null;
        }


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
            // ����ü ���� �� ��ǥ ����
            GameObject pouBall = Instantiate(pouBallPrefab, attackPoint.position, Quaternion.identity);
            pouBall.GetComponent<PouBall>().SetTarget(enemyTarget.position);  // ��ǥ ����
            Debug.Log("�����߽�~!");
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag) && canAttack)  // ���� ������ ���� �߻�
        {
            enemyTarget = other.transform;  // ���� ���� ������� ����
            FireSkill();  // ���� �߻�
            canAttack = false;  // ���� �Ұ���

            // ��Ÿ�� ���
            StartCoroutine(SkillCooldown());  // ��Ÿ�� ���
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
        if (other.CompareTag(targetTag))
        {
            enemyTarget = null;  // �� ���� ����
        }
    }

    public IEnumerator AttackDelay(Enemy enemy)
    {
        canAttack = false;

        if (animator != null)
        {
            animator.SetTrigger("Shoot"); // "Attack" Ʈ���� �ߵ�
        }


        yield return new WaitForSeconds(1);
        canAttack = true;
    }
}
