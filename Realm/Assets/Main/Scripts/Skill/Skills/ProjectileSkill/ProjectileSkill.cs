using UnityEngine;
using System.Collections;

public class ProjectileSkill : Skill
{
    
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    private ProjectileSkillStat projectileStats;
    private Coroutine fireCoroutine;
    private bool isSkillActive = false;

    private void Awake()
    {
        projectileStats = (ProjectileSkillStat)skillStat;
        
    }

    protected override void UseSkill()
    {
        if (!isSkillActive)
        {
            isSkillActive = true;
            fireCoroutine = StartCoroutine(FireRoutine());
        }
    }

    protected virtual void StopSkill()
    {
        if (isSkillActive)
        {
            isSkillActive = false;
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
        }
    }

    private IEnumerator FireRoutine()
    {
        while (isSkillActive)
        {
            // 스탯 가져오기
            float shotInterval = projectileStats.GetStatValue<float>(SkillStatType.ShotInterval);
            float innerInterval = projectileStats.GetStatValue<float>(SkillStatType.InnerInterval);
            int projectileCount = Mathf.RoundToInt(projectileStats.GetStatValue<float>(SkillStatType.ProjectileCount));

            // 투사체 데이터 준비
            ProjectileData projectileData = new ProjectileData
            {
                Damage = projectileStats.GetStatValue<float>(SkillStatType.Damage),
                Speed = projectileStats.GetStatValue<float>(SkillStatType.ProjectileSpeed),
                Range = projectileStats.GetStatValue<float>(SkillStatType.ProjectileRange),
                PierceCount = Mathf.RoundToInt(projectileStats.GetStatValue<float>(SkillStatType.PierceCount)),
                IsHoming = projectileStats.GetStatValue<bool>(SkillStatType.IsHoming),
                HomingRange = projectileStats.GetStatValue<float>(SkillStatType.HomingRange)
            };

            // 여러 투사체 발사
            for (int i = 0; i < projectileCount; i++)
            {
                FireProjectile(projectileData);

                // 투사체 사이의 간격이 있다면 대기
                if (innerInterval > 0 && i < projectileCount - 1)
                {
                    yield return new WaitForSeconds(innerInterval);
                }
            }

            // 다음 발사까지 대기
            yield return new WaitForSeconds(shotInterval);
        }
    }

    private void FireProjectile(ProjectileData data)
    {
        // 여기서 발사 각도나 패턴을 추가하면 좋을듯
        Projectile projectile = PoolManager.Instance.Spawn<Projectile>
            (projectilePrefab.gameObject, firePoint.position, firePoint.rotation);
        projectile.Initialize(data);
    }

    // 스킬 강화 시
    public override void LevelUp()
    {
        base.LevelUp();

        Debug.Log($"Level {skillLevel} Stats:");
        Debug.Log($"Damage: {projectileStats.GetStatValue<float>(SkillStatType.Damage)}");
        Debug.Log($"ProjectileCount: {projectileStats.GetStatValue<float>(SkillStatType.ProjectileCount)}");
        Debug.Log($"IsHoming: {projectileStats.GetStatValue<bool>(SkillStatType.IsHoming)}");
    }

    private void OnDisable()
    {
        StopSkill();
    }
}

// 투사체 데이터 구조체 (구조체로 데이터를 감싸 투사체에 넘겨주는 방식)
public struct ProjectileData
{
    public float Damage;
    public float Speed;
    public float Range;
    public int PierceCount;
    public bool IsHoming;
    public float HomingRange;
}