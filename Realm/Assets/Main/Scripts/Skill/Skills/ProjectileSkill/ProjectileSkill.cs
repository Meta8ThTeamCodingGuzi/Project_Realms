using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ProjectileSkillStat))]
public class ProjectileSkill : Skill
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    private ProjectileSkillStat projectileStats;

    public void Start()
    {
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{gameObject.name}: projectilePrefab�� �Ҵ���� �ʾҽ��ϴ�!");
        }

        if (firePoint == null)
        {
            // firePoint�� ������ �ڵ����� ����
            GameObject firePointObj = new GameObject("FirePoint");
            firePoint = firePointObj.transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = Vector3.zero;
            Debug.Log($"{gameObject.name}: FirePoint�� �ڵ����� �����Ǿ����ϴ�.");
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        if (skillStat != null)
        {
            projectileStats = (ProjectileSkillStat)skillStat;
            projectileStats.InitializeStats();
            Debug.Log($"{gameObject.name}: ProjectileSkill �ʱ�ȭ �Ϸ�");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: skillStat�� null�Դϴ�!");
        }
    }

    protected override void UseSkill()
    {
        StartCoroutine(FireSequence());
    }

    private IEnumerator FireSequence()
    {
        if (projectileStats == null)
        {
            Debug.LogError($"{gameObject.name}: projectileStats�� null�Դϴ�!");
            yield break;
        }

        Debug.Log($"Shot Interval: {projectileStats.GetStatValue<float>(SkillStatType.ShotInterval)}");
        Debug.Log($"Inner Interval: {projectileStats.GetStatValue<float>(SkillStatType.InnerInterval)}");
        Debug.Log($"Projectile Count: {projectileStats.GetStatValue<int>(SkillStatType.ProjectileCount)}");
        Debug.Log($"Damage: {projectileStats.GetStatValue<float>(SkillStatType.Damage)}");

        float shotInterval = projectileStats.GetStatValue<float>(SkillStatType.ShotInterval);
        float innerInterval = projectileStats.GetStatValue<float>(SkillStatType.InnerInterval);
        int projectileCount = projectileStats.GetStatValue<int>(SkillStatType.ProjectileCount);

        ProjectileData projectileData = new ProjectileData
        {
            Damage = projectileStats.GetStatValue<float>(SkillStatType.Damage),
            Speed = projectileStats.GetStatValue<float>(SkillStatType.ProjectileSpeed),
            Range = projectileStats.GetStatValue<float>(SkillStatType.ProjectileRange),
            PierceCount = projectileStats.GetStatValue<int>(SkillStatType.PierceCount),
            IsHoming = projectileStats.GetStatValue<int>(SkillStatType.IsHoming) <=
                       projectileStats.GetStatValue<int>(SkillStatType.HomingLevel) ? false : true,
            HomingRange = projectileStats.GetStatValue<float>(SkillStatType.HomingRange),
        };

        for (int i = 0; i < projectileCount; i++)
        {
            FireProjectile(projectileData);

            if (innerInterval > 0 && i < projectileCount - 1)
            {
                yield return new WaitForSeconds(innerInterval);
            }
        }
    }

    private void FireProjectile(ProjectileData data)
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{gameObject.name}: projectilePrefab�� �����ϴ�!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError($"{gameObject.name}: firePoint�� �����ϴ�!");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 direction = (targetPoint - transform.position).normalized;
            direction.y = 0;

            GameManager.Instance.player.transform.rotation = Quaternion.LookRotation(direction);
            firePoint.rotation = transform.rotation;

            // Ǯ���� ������Ÿ���� �������� ���� �������� ��ȿ���� �ѹ� �� Ȯ��
            if (projectilePrefab != null && projectilePrefab.gameObject != null)
            {
                try
                {
                    Projectile projectile = PoolManager.Instance.Spawn<Projectile>(
                        projectilePrefab.gameObject,
                        firePoint.position,
                        firePoint.rotation);

                    if (projectile != null)
                    {
                        projectile.Initialize(data);

                        // Despawn ���� projectile�� ���� ��ȿ���� Ȯ��
                        if (projectile != null && projectile.gameObject != null)
                        {
                            PoolManager.Instance.Despawn<Projectile>(
                                projectile,
                                skillStat.GetStatValue<float>(SkillStatType.Duration));
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"{gameObject.name}: ������Ÿ�� ���� �� ���� �߻�: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"{gameObject.name}: ������Ÿ�� �������� ��ȿ���� �ʽ��ϴ�!");
            }
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
        PrintAllStats();
    }
}

public struct ProjectileData
{
    public float Damage;
    public float Speed;
    public float Range;
    public int PierceCount;
    public bool IsHoming;
    public float HomingRange;
}