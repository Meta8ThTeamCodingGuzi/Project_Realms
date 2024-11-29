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
        if (skillStat == null)
        {
            Initialize();
        }
        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError($"{gameObject.name}: projectilePrefab이 할당되지 않았습니다!");
        }

        if (firePoint == null)
        {
            // firePoint가 없으면 자동으로 생성
            GameObject firePointObj = new GameObject("FirePoint");
            firePoint = firePointObj.transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = Vector3.zero;
            Debug.Log($"{gameObject.name}: FirePoint가 자동으로 생성되었습니다.");
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        if (skillStat != null)
        {
            projectileStats = (ProjectileSkillStat)skillStat;
            projectileStats.InitializeStats();
            Debug.Log($"{gameObject.name}: ProjectileSkill 초기화 완료");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: skillStat이 null입니다!");
        }
    }

    protected override void UseSkill()
    {
        StartCoroutine(FireSequence());
    }

    private IEnumerator FireSequence()
    {
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
            HomingRange = projectileStats.GetStatValue<float>(SkillStatType.HomingRange)
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
            Debug.LogError($"{gameObject.name}: projectilePrefab이 없습니다!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError($"{gameObject.name}: firePoint가 없습니다!");
            return;
        }

        Projectile projectile = PoolManager.Instance.Spawn<Projectile>
            (projectilePrefab.gameObject, firePoint.position, firePoint.rotation);

        if (projectile != null)
        {
            projectile.Initialize(data);
        }
        else
        {
            Debug.LogError($"{gameObject.name}: 발사체 생성 실패!");
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