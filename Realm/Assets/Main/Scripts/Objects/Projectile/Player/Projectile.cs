using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticle;
    protected ProjectileData data;
    private Vector3 startPosition;
    private float distanceTraveled;
    private int remainingPierceCount;
    private Transform target;
    private bool isOwnerPlayer;

    public void Initialize(ProjectileData data)
    {
        this.data = data;
        startPosition = transform.position;
        remainingPierceCount = data.PierceCount;
        isOwnerPlayer = data.owner is Player;

        if (data.IsHoming)
            FindTarget();
    }

    private void Update()
    {
        ProjectileMove();
    }

    private void ProjectileMove()
    {
        if (data.IsHoming && target != null)
        {
            Unit targetUnit = target.GetComponent<Unit>();
            if (!targetUnit.IsAlive)
            {
                target = null;
            }
            else
            {
                Vector3 direction = (target.position - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * data.Speed);
            }
        }

        transform.position += transform.forward * data.Speed * Time.deltaTime;
        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        if (distanceTraveled >= data.Range)
        {
            PlayHitParticle();
            PoolManager.Instance.Despawn(this);
        }
    }

    private void PlayHitParticle()
    {
        if (hitParticle != null)
        {
            ParticleSystem hp = PoolManager.Instance.Spawn<ParticleSystem>(hitParticle.gameObject, transform.position, Quaternion.identity);
            hp.Play();
            PoolManager.Instance.Despawn(hp, 1.5f);
        }
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, data.HomingRange);
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<Unit>(out Unit targetUnit))
                continue;

            bool isValidTarget = false;
            if (isOwnerPlayer && targetUnit is Monster)
                isValidTarget = true;
            else if (!isOwnerPlayer && targetUnit is Player && !(data.owner is Pet))
                isValidTarget = true;
            else if (data.owner is Pet && targetUnit is Monster)
                isValidTarget = true;

            if (isValidTarget)
            {
                float distance = Vector3.Distance(transform.position, targetUnit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = targetUnit.transform;
                }
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Projectile collision with: {other.gameObject.name}");
        Debug.Log($"Owner is: {data.owner.GetType().Name}");

        // 발사체가 자신의 주인과 충돌하면 무시
        if (other.gameObject == data.owner.gameObject)
        {
            Debug.Log("Collision with owner, ignoring");
            return;
        }

        // 펫이 쏜 발사체가 플레이어와 충돌하면 무시
        if (data.owner is Pet && other.gameObject == GameManager.Instance.player.gameObject)
        {
            Debug.Log("Pet projectile hit player, ignoring");
            return;
        }

        // Unit이 아닌 대상과 충돌하면 무시
        if (!other.TryGetComponent<Unit>(out Unit targetUnit))
        {
            Debug.Log("Target is not a Unit, ignoring");
            return;
        }

        Debug.Log($"Target Unit type: {targetUnit.GetType().Name}");

        bool isValidTarget = false;
        if ((data.owner is Player || data.owner is Pet) && targetUnit is Monster)
        {
            Debug.Log("Valid target: Player/Pet hitting Monster");
            isValidTarget = true;
        }
        else if (!(data.owner is Player || data.owner is Pet) && targetUnit is Player)
        {
            Debug.Log("Valid target: Monster hitting Player");
            isValidTarget = true;
        }
        else
        {
            Debug.Log("Invalid target combination");
        }

        if (isValidTarget)
        {
            Debug.Log($"Dealing damage to: {targetUnit.gameObject.name}");
            targetUnit.TakeDamage(data.Damage);
            PlayHitParticle();

            remainingPierceCount--;
            if (remainingPierceCount <= 0)
            {
                PoolManager.Instance.Despawn(this);
            }
        }
    }
}