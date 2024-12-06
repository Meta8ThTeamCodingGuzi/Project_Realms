using Unity.Android.Types;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticle;
    private ProjectileData data;
    private Vector3 startPosition;
    private float distanceTraveled;
    private int remainingPierceCount;
    private Transform target;

    public void Initialize(ProjectileData data)
    {
        this.data = data;
        startPosition = transform.position;
        remainingPierceCount = data.PierceCount;

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
            Monster targetMonster = target.GetComponent<Monster>();
            // Ÿ���� ��Ȱ��ȭ�Ǿ��ų� �ı��Ǿ��ٸ� �� �̻� �������� ����
            if (!targetMonster.IsAlive)
            {
                target = null;
            }
            else
            {
                // ���� ����
                Vector3 direction = (target.position - transform.position).normalized;
                transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * data.Speed);
            }
        }

        // �̵�
        transform.position += transform.forward * data.Speed * Time.deltaTime;
        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        // �ִ� ��Ÿ� üũ
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
        // ���� ����� �� ã��
        Collider[] colliders = Physics.OverlapSphere(transform.position, data.HomingRange);
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<Monster>(out Monster monster))
            {
                float distance = Vector3.Distance(transform.position, monster.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = monster.transform;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Monster>(out Monster monster))
        {
            // ������ ó��
            monster.TakeDamage(data.Damage);

            PlayHitParticle();

            // ���� ó��
            remainingPierceCount--;
            if (remainingPierceCount <= 0)
            {
                PoolManager.Instance.Despawn(this);
            }
        }
    }
}