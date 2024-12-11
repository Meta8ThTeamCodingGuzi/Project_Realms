using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pet : MonoBehaviour
{
    private Player targetPlayer;  
    private Transform enemyTarget;
    public Transform attackPoint; 
    private bool canAttack = true;
    private NavMeshAgent agent;
    private Animator animator;
    [Header("펫과나의 거리두기"), Range(1f, 12f)]
    public float playDis;
    [Header("펫과적의 거리두기"), Range(1f, 12f)]
    public float EnemyDis;
    [Header("텔포 호출거리"), Range(15f, 25f)]
    public float playerTellme = 15f;

    [SerializeField] private Transform FirePoint;

    public GameObject tellEffect;

    public ProjectileSkill pouBall;

    private ProjectileSkill poupouball;


    public void Initialize(Player player)
    {
        targetPlayer = player;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.stoppingDistance = 8;  // 플레이어와의 멈춤 거리
        poupouball = Instantiate(pouBall, transform);
        poupouball.Initialize(player);
        poupouball.firePoint = FirePoint;
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

    // 스킬 발사 로직
    private void FireSkill()
    {
        if (attackPoint != null)
        {
            poupouball.TryUseSkill();
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

    private IEnumerator SkillCooldown()
    {
        yield return new WaitForSeconds(1);
        canAttack = true;  // 쿨타임 종료 후 공격 가능
        Debug.Log("쿨타임 끝, 다시 공격 가능!");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster))
        {
            enemyTarget = null;  // 적 추적 중지
        }
    }
}
