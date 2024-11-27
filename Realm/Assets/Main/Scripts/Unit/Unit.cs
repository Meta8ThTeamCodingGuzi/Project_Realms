using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEditor;

public abstract class Unit : MonoBehaviour, IDamageable, IMovable
{
    protected NavMeshAgent agent;
    protected ICharacterStats characterStats;
    public ICharacterStats CharacterStats => characterStats;


    protected float lastAttackTime;
    protected Coroutine attackCoroutine;

 

    protected virtual void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            agent = gameObject.AddComponent<NavMeshAgent>();
        }

        characterStats = GetComponent<ICharacterStats>();
        if (characterStats == null)
        {
            Debug.LogError($"이색기 스탯 안달림 {gameObject.name}");
        }

        characterStats.InitializeStats();

        UpdateMoveSpeed();
    }


    #region 전투관련
    public virtual bool IsAlive => characterStats.GetStatValue(StatType.Health) > 0;
    public virtual void Attack(Unit target)
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(AttackRoutine(target));
    }


    public virtual void StopAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    protected virtual IEnumerator AttackRoutine(Unit target)
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
            else
            {
                // 대상이 공격 범위를 벗어났을 경우
                MoveTo(target.transform.position);
            }

            yield return new WaitForSeconds(0.1f);
        }

        attackCoroutine = null;
    }
  

    public virtual bool CanAttack(Unit target)
    {
        if (target == null || !target.IsAlive || !IsAlive) return false;

        float attackRange = characterStats.GetStatValue(StatType.AttackRange);
        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        return distanceToTarget <= attackRange;
    }

    protected virtual void PerformAttack(Unit target)
    {
        if (target != null && target.IsAlive)
        {
            float damage = characterStats.GetStatValue(StatType.Damage);
            target.TakeDamage(damage);
            OnAttackPerformed(target);
        }
    }

    protected virtual void OnAttackPerformed(Unit target)
    {
        // 여기에 공격 이펙트 , 애니메이션등 들어가면 될듯.
        Debug.Log($"{gameObject.name}이(가) {target.gameObject.name}을(를) 공격했습니다.");
    }

    public virtual void TakeDamage(float damage)
    {
        if (!IsAlive) return;

        float defense = characterStats.GetStatValue(StatType.Defense);

        // 최종데미지 = 데미지 * (100 / (100 + 방어력))
        float damageMultiplier = 100f / (100f + defense);
        float finalDamage = damage * damageMultiplier;

        // 최소 1의 데미지는 들어가도록 설정했읍니다.
        finalDamage = Mathf.Max(1f, finalDamage);

        StatModifier healthMod = new StatModifier(-finalDamage, StatModifierType.Flat);
        characterStats.AddModifier(StatType.Health, healthMod);
    }
    #endregion

    #region 움직임 관련
    public virtual float MoveSpeed
    {
        get => agent.speed;
        protected set => agent.speed = value;
    }

    public virtual bool IsMoving => agent != null && agent.hasPath && agent.velocity.sqrMagnitude > 0.01f;

    public virtual void MoveTo(Vector3 destination)
    {
        if (agent != null && agent.isActiveAndEnabled && IsAlive)
        {
            agent.SetDestination(destination);
        }
    }

    public virtual void StopMoving()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.ResetPath();
        }
    }

    protected virtual void UpdateMoveSpeed()
    {
        if (characterStats != null)
        {
            MoveSpeed = characterStats.GetStatValue(StatType.MoveSpeed);
        }
    }
    public virtual bool HasReachedDestination()
    {
        // 경로가 없거나 에이전트가 비활성화된 경우
        if (agent == null || !agent.isActiveAndEnabled) return false;

        // 경로가 유효하지 않은 경우
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid) return false;

        // 남은 거리가 거의 없고 경로가 유효한 경우
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    #endregion

    #region 기즈모 관련
#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (characterStats != null)
        {
            float attackRange = characterStats.GetStatValue(StatType.AttackRange);
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);
            Gizmos.DrawSphere(transform.position, attackRange);
        }
    }

    private void OnDrawGizmos()
    {
        if (characterStats != null)
        {
            float attackRange = characterStats.GetStatValue(StatType.AttackRange);
            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, attackRange);

            if (UnityEditor.Selection.activeGameObject == gameObject)
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
                Gizmos.DrawSphere(transform.position, attackRange);
            }
        }
    }

#endif
    #endregion
}
