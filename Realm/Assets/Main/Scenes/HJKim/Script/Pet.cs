using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    private Transform playerTarget;  // 플레이어를 따라가는 목표
    private Transform enemyTarget;  // 적 목표
    public string targetTag = "Enemy";  // 적 태그 설정
    public GameObject pouBallPrefab;  // 투사체 프리팹
    public Transform attackPoint;  // 투사체 발사 위치
    private bool canAttack = true;
    private NavMeshAgent agent;
    private Animator animator;
    [Header("펫과나의 거리두기"), Range(1f, 12f)]
    public float playDis;
    [Header("펫과적의 거리두기"), Range(1f, 12f)]
    public float EnemyDis;
    [Header("텔포 호출거리"), Range(15f, 25f)]
    public float playerTellme = 15f;

    private void Start()
    {
        Initialize();
    }

    // 마지막 스킬 발사 시간
    private void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerTarget = GameManager.Instance.player.transform;
        agent.stoppingDistance = 8;  // 플레이어와의 멈춤 거리
    }

    void Update()
    {
        Move();
        SetAnim();
        PlayerTell();
    }

    // 이동 로직: 적이 있으면 적 추적, 없으면 플레이어 추적
    private void Move()
    {
        if (enemyTarget != null)  // 적이 감지되었으면
        {
            agent.stoppingDistance = EnemyDis;
            agent.SetDestination(enemyTarget.position);
            transform.LookAt(enemyTarget.position);
        }
        else if (playerTarget != null)  // 적이 없으면 플레이어 추적
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
    // 애니메이션 설정: 움직임에 따라 애니메이션 변경
    private void SetAnim()
    {
        if (agent.velocity.sqrMagnitude < 0.01f)  // 이동하지 않을 때
        {
            animator.SetBool("isRunning", false);  // IDLE 애니메이션
        }
        else  // 이동 중일 때
        {
            animator.SetBool("isRunning", true);  // RUNNING 애니메이션
        }
    }

    // 스킬 발사 로직
    private void FireSkill()
    {
        if (attackPoint != null)
        {
            // 투사체 생성 및 목표 설정
            GameObject pouBall = Instantiate(pouBallPrefab, attackPoint.position, Quaternion.identity);
            pouBall.GetComponent<PouBall>().SetTarget(enemyTarget.position);  // 목표 설정
            Debug.Log("히히발싸~!");
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(targetTag) && canAttack)  // 공격 가능할 때만 발사
        {
            enemyTarget = other.transform;  // 적을 추적 대상으로 설정
            FireSkill();  // 공격 발사
            canAttack = false;  // 공격 불가능

            // 쿨타임 대기
            StartCoroutine(SkillCooldown());  // 쿨타임 대기
        }
    }

    // 쿨타임 대기 코루틴
    private IEnumerator SkillCooldown()
    {
        yield return new WaitForSeconds(1);
        canAttack = true;  // 쿨타임 종료 후 공격 가능
        Debug.Log("쿨타임 끝, 다시 공격 가능!");
    }


    // 적이 범위를 벗어났을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            enemyTarget = null;  // 적 추적 중지
        }
    }

    public IEnumerator AttackDelay(Enemy enemy)
    {
        canAttack = false;

        if (animator != null)
        {
            animator.SetTrigger("Shoot"); // "Attack" 트리거 발동
        }


        yield return new WaitForSeconds(1);
        canAttack = true;
    }
}
