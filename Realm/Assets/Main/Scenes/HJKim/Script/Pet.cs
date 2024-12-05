using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    public Transform playerTarget;  // �÷��̾� ��ǥ
    private Transform enemyTarget;  // �� ��ǥ
    NavMeshAgent agent;
    public string targetTag = "Enemy";  // "Enemy" �±׸� ���� ������Ʈ�� ����
    private Animator animator;
    private bool isMoving;


    public void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 8;

    }

    public void Update()
    {
        Move();
        SetAnim();
    }

    // ���� �̵� ���� ����
    // 1. �ֺ��� ���� ������ �÷��̾ ���󰣴�.
    // 2. �ֺ��� ���� ������ ���� ���󰣴�.
    private void Move()
    {
        //if (agent.remainingDistance <= agent.stoppingDistance)
        //{
        //    animator.SetBool("isrunning", false);  // �ִϸ��̼� ����
        //}
        if (enemyTarget != null)  // ���� �����Ǿ����� ���� ����
        {
            agent.stoppingDistance = 5;
            agent.SetDestination(enemyTarget.position);
            //animator.SetBool("isRunning", true);
        }
        else if (playerTarget != null)  // ���� ���ٸ� �÷��̾ ����
        {
            agent.stoppingDistance = 8;
            agent.SetDestination(playerTarget.position);
            //animator.SetBool("isRunning", true);
        }
    }

    // ���� �ִϸ��̼� ���� ����
    // 1. ��ǥ������ Ư�� ������ �����ϸ� IDLE
    // 2. ��ǥ�� �����ϸ� �̵��ϰ� ������ MOVE
    private void SetAnim()
    {
        // ���� ��ǥ���������� ���� <= ���� ������ϴ� ���� 
        if (agent.velocity.sqrMagnitude < 0.01f)
        {
            // IDLE �ִϸ��̼�            
            animator.SetBool("isRunning", false);  // �ִϸ��̼� ����
        }
        else
        {
            // MOVE �ִϸ��̼�
            animator.SetBool("isRunning", true);
        }
    }

    private void AttackAnim()
    {
        animator.SetInteger("Attack", true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))  // �� �±׸� ���� ������Ʈ�� �ݶ��̴��� ������
        {
            enemyTarget = other.transform;  // target�� ������ ����
            agent.SetDestination(enemyTarget.position);  // ���� �����ϱ� ����
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))  // ���� �������� �����
        {
            enemyTarget = null;  // �� ���� ����
        }
    }



}
