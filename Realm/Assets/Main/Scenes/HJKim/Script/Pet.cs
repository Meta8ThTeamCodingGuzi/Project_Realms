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

    // ������ ��ų �߻� �ð�

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.stoppingDistance = 8;  // �÷��̾���� ���� �Ÿ�
    }

    void Update()
    {
        Move();
        SetAnim();

    }

    // �̵� ����: ���� ������ �� ����, ������ �÷��̾� ����
    private void Move()
    {
        if (enemyTarget != null)  // ���� �����Ǿ�����
        {
            agent.stoppingDistance = 14;
            agent.SetDestination(enemyTarget.position);
        }
        else if (playerTarget != null)  // ���� ������ �÷��̾� ����
        {
            agent.stoppingDistance = 15;
            agent.SetDestination(playerTarget.position);
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
