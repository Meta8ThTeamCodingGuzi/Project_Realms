using UnityEngine;

public class Projectile : MonoBehaviour
{
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
            // ���� ����
            Vector3 direction = (target.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);
        }

        // �̵�
        transform.position += transform.forward * data.Speed * Time.deltaTime;
        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        // �ִ� ��Ÿ� üũ
        if (distanceTraveled >= data.Range)
        {
            PoolManager.Instance.Despawn(this);
        }
    }

    private void FindTarget()
    {
        // ���� ����� �� ã��
        Collider[] colliders = Physics.OverlapSphere(transform.position, data.HomingRange);
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = collider.transform;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // ������ ó��
            if (other.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(data.Damage);
            }

            // ���� ó��
            remainingPierceCount--;
            if (remainingPierceCount < 0)
            {
                PoolManager.Instance.Despawn(this);
            }
        }
    }
}