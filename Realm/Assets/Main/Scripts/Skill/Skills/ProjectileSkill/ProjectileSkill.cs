using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ProjectileSkillStat))]
public class ProjectileSkill : Skill
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] public Transform firePoint { get; set; }

    private ProjectileSkillStat projectileStats;
    private Vector3? cachedTargetDirection;

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

        firePoint = Owner.transform?.Find("FirePoint");

        if (firePoint == null)
        {
            // firePoint�� ������ �ڵ����� ����
            GameObject firePointObj = new GameObject("FirePoint");
            firePoint = firePointObj.transform;
            firePoint.SetParent(Owner.transform);
            firePoint.localPosition = new Vector3(0, 1f, 0.5f);
            Debug.Log($"{gameObject.name}: FirePoint�� �ڵ����� �����Ǿ����ϴ�.");
        }
    }

    public override void Initialize(Unit owner)
    {
        base.Initialize(owner);

        if (skillStat != null)
        {
            projectileStats = (ProjectileSkillStat)skillStat;
            projectileStats.InitializeStats();
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

    protected virtual IEnumerator FireSequence()
    {
        if (projectileStats == null)
        {
            Debug.LogError($"{gameObject.name}: projectileStats�� null�Դϴ�!");
            yield break;
        }

        cachedTargetDirection = GetTargetDirection();
        if (!cachedTargetDirection.HasValue)
        {
            yield break;
        }

        isSkillInProgress = true;

        float shotInterval = projectileStats.GetStatValue<float>(SkillStatType.ShotInterval);
        float innerInterval = projectileStats.GetStatValue<float>(SkillStatType.InnerInterval);
        int projectileCount = projectileStats.GetStatValue<int>(SkillStatType.ProjectileCount);

        ProjectileData projectileData = new ProjectileData
        {
            owner = Owner,
            Damage = projectileStats.GetStatValue<float>(SkillStatType.Damage),
            Speed = projectileStats.GetStatValue<float>(SkillStatType.ProjectileSpeed),
            Range = projectileStats.GetStatValue<float>(SkillStatType.ProjectileRange),
            PierceCount = projectileStats.GetStatValue<int>(SkillStatType.PierceCount),
            IsHoming = projectileStats.GetStatValue<int>(SkillStatType.IsHoming) <=
                       projectileStats.GetStatValue<int>(SkillStatType.HomingLevel) ? false : true,
            HomingRange = projectileStats.GetStatValue<float>(SkillStatType.HomingRange),
        };

        if (Owner is Player)
        {
            Owner.transform.rotation = Quaternion.LookRotation(cachedTargetDirection.Value);
            firePoint.rotation = Owner.transform.rotation;
        }
        else
        {
            Owner.transform.LookAt(Owner.Target.transform);
            firePoint.LookAt(Owner.Target.transform);
        }

        for (int i = 0; i < projectileCount; i++)
        {
            if (Owner.Animator != null)
            {
                Owner.Animator.SetTrigger("Attack");
            }
            FireProjectile(projectileData);
            if (innerInterval > 0 && i < projectileCount - 1)
            {
                yield return new WaitForSeconds(innerInterval);
            }
        }

        Owner.Animator.SetTrigger("Idle");
        isSkillInProgress = false;

        cachedTargetDirection = null;
    }

    private Vector3? GetTargetDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 direction = (targetPoint - transform.position).normalized;
            direction.y = 0;
            return direction;
        }
        return null;
    }

    protected virtual void FireProjectile(ProjectileData data)
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

        if (projectilePrefab != null && projectilePrefab.gameObject != null && cachedTargetDirection.HasValue)
        {
            try
            {
                Projectile projectile = PoolManager.Instance.Spawn<Projectile>(
                    projectilePrefab.gameObject,
                    firePoint.position,
                    Quaternion.LookRotation(cachedTargetDirection.Value));

                if (projectile != null)
                {
                    projectile.Initialize(data);
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

    public override void LevelUp()
    {
        base.LevelUp();
        PrintAllStats();
    }
}

public struct ProjectileData
{
    public Unit owner;
    public float Damage;
    public float Speed;
    public float Range;
    public int PierceCount;
    public bool IsHoming;
    public float HomingRange;
}