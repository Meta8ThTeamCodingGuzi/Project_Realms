using UnityEngine;

public class BowSkill : WeaponSkill
{
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Projectile arrowPrefab;
    private float lastFireTime;

    public override void Initialize()
    {
        if (arrowSpawnPoint == null)
        {
            arrowSpawnPoint = transform.Find("ArrowSpawnPoint");
            if (arrowSpawnPoint == null)
            {
                arrowSpawnPoint = new GameObject("ArrowSpawnPoint").transform;
                arrowSpawnPoint.SetParent(transform);
                arrowSpawnPoint.localPosition = Vector3.forward;
            }
        }
    }

    protected override void UseSkill()
    {
        // ���� �ӵ��� ���� �߻� ���� üũ
        if (Time.time - lastFireTime >= 1f / GetPlayerAttackSpeed())
        {
            FireArrow();
            lastFireTime = Time.time;
        }
    }

    private void FireArrow()
    {
        if (arrowPrefab != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPoint = ray.GetPoint(distance);

                float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
                if (distanceToTarget > GetAttackRange())
                {
                    targetPoint = transform.position + (targetPoint - transform.position).normalized * GetAttackRange();
                }

                Vector3 direction = (targetPoint - arrowSpawnPoint.position).normalized;

                Projectile arrow = PoolManager.Instance.Spawn<Projectile>(
                    arrowPrefab.gameObject,
                    arrowSpawnPoint.position,
                    Quaternion.LookRotation(direction));

                if (arrow != null)
                {
                    // �÷��̾��� �⺻ ���ݷ� + ��ų�� �߰� ������ ���
                    float totalDamage = GetPlayerDamage() *
                        (1 + skillStat.GetStatValue<float>(SkillStatType.Damage) / 100f);

                    ProjectileData data = new ProjectileData
                    {
                        Damage = totalDamage,
                        Speed = skillStat.GetStatValue<float>(SkillStatType.ProjectileSpeed),
                        Range = skillStat.GetStatValue<float>(SkillStatType.ProjectileRange)
                    };

                    arrow.Initialize(data);
                }
            }
        }
    }

    protected override void OnWeaponHit(Collider other)
    {
        // Ȱ�� �߻�ü�� �������� ó��
    }
}