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
            // 유도 로직
            Vector3 direction = (target.position - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);
        }

        // 이동
        transform.position += transform.forward * data.Speed * Time.deltaTime;
        distanceTraveled = Vector3.Distance(startPosition, transform.position);

        // 최대 사거리 체크
        if (distanceTraveled >= data.Range)
        {
            PoolManager.Instance.Despawn(this);
        }
    }

    private void FindTarget()
    {
        // 가장 가까운 적 찾기
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
            // 데미지 처리
            monster.TakeDamage(data.Damage);

            // 관통 처리
            remainingPierceCount--;
            if (remainingPierceCount <= 0)
            {
                PoolManager.Instance.Despawn(this);
            }
        }
    }
}