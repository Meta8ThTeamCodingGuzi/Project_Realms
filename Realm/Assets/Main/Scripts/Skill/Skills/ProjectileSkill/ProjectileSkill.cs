using UnityEngine;
using System.Collections;

public class ProjectileSkill : Skill
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    private ProjectileSkillStat projectileStats;

    public override void Initialize()
    {
        base.Initialize();
        projectileStats = (ProjectileSkillStat)skillStat;
        projectileStats.InitializeStats();
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
        Projectile projectile = PoolManager.Instance.Spawn<Projectile>
            (projectilePrefab.gameObject, firePoint.position, firePoint.rotation);
        projectile.Initialize(data);
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