using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    public Transform playerTarget;  // 플레이어 목표
    private Transform enemyTarget;  // 적 목표
    NavMeshAgent agent;
    public string targetTag = "Enemy";  // "Enemy" 태그를 가진 오브젝트를 추적
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

    // 펫의 이동 관련 로직
    // 1. 주변에 적이 없으면 플레이어를 따라간다.
    // 2. 주변에 적이 있으면 적을 따라간다.
    private void Move()
    {
        //if (agent.remainingDistance <= agent.stoppingDistance)
        //{
        //    animator.SetBool("isrunning", false);  // 애니메이션 멈춤
        //}
        if (enemyTarget != null)  // 적이 감지되었으면 적을 추적
        {
            agent.stoppingDistance = 5;
            agent.SetDestination(enemyTarget.position);
            //animator.SetBool("isRunning", true);
        }
        else if (playerTarget != null)  // 적이 없다면 플레이어를 추적
        {
            agent.stoppingDistance = 8;
            agent.SetDestination(playerTarget.position);
            //animator.SetBool("isRunning", true);
        }
    }

    // 펫의 애니메이션 관련 로직
    // 1. 목표지점의 특정 범위에 도달하면 IDLE
    // 2. 목표를 추적하며 이동하고 있으면 MOVE
    private void SetAnim()
    {
        // 나와 목표지점까지의 길이 <= 내가 멈춰야하는 길이 
        if (agent.velocity.sqrMagnitude < 0.01f)
        {
            // IDLE 애니메이션            
            animator.SetBool("isRunning", false);  // 애니메이션 멈춤
        }
        else
        {
            // MOVE 애니메이션
            animator.SetBool("isRunning", true);
        }
    }

    private void AttackAnim()
    {
        animator.SetInteger("Attack", true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))  // 적 태그를 가진 오브젝트가 콜라이더에 들어오면
        {
            enemyTarget = other.transform;  // target을 적으로 설정
            agent.SetDestination(enemyTarget.position);  // 적을 추적하기 시작
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))  // 적이 범위에서 벗어나면
        {
            enemyTarget = null;  // 적 추적 중지
        }
    }



}
