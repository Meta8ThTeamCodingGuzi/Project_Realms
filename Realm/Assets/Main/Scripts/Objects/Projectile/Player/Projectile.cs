 using Unity.Android.Types;
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
        this.transform.localScale = Vector3.one * data.Scale;

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
        ParticleSystem hp = PoolManager.Instance.Spawn<ParticleSystem>(hitParticle.gameObject, transform.position, Quaternion.identity);
        hp.Play();
        PoolManager.Instance.Despawn(hp, 1.5f);
    }

    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, data.HomingRange);
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent<Unit>(out Unit targetUnit))
                continue;

            if ((isOwnerPlayer && targetUnit is Monster) || (!isOwnerPlayer && targetUnit is Player))
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
        if (!other.TryGetComponent<Unit>(out Unit targetUnit))
            return;

        if ((isOwnerPlayer && targetUnit is Monster) || (!isOwnerPlayer && targetUnit is Player))
        {
            targetUnit.TakeDamage(data.Damage);

            PlayHitParticle();

            remainingPierceCount--;
            if (remainingPierceCount == 0)
            {
                PoolManager.Instance.Despawn(this);
            }
        }
    }
}