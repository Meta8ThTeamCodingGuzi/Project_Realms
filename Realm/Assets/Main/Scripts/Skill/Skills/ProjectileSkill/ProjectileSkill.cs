using UnityEngine;
using System.Collections;

public class ProjectileSkill : Skill
{
   
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    private ProjectileSkillStat projectileStats;
    private Coroutine fireCoroutine;
    private bool isSkillActive = false;

    public override void Initialize()
    {
        base.Initialize();
        projectileStats = (ProjectileSkillStat)skillStat;
        projectileStats.InitializeStats();
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
            // ���� ��������
            float shotInterval = projectileStats.GetStatValue<float>(SkillStatType.ShotInterval);
            float innerInterval = projectileStats.GetStatValue<float>(SkillStatType.InnerInterval);
            int projectileCount = Mathf.RoundToInt(projectileStats.GetStatValue<float>(SkillStatType.ProjectileCount));

            // ����ü ������ �غ�
            ProjectileData projectileData = new ProjectileData
            {
                Damage = projectileStats.GetStatValue<float>(SkillStatType.Damage),
                Speed = projectileStats.GetStatValue<float>(SkillStatType.ProjectileSpeed),
                Range = projectileStats.GetStatValue<float>(SkillStatType.ProjectileRange),
                PierceCount = Mathf.RoundToInt(projectileStats.GetStatValue<float>(SkillStatType.PierceCount)),
                IsHoming = projectileStats.GetStatValue<bool>(SkillStatType.IsHoming),
                HomingRange = projectileStats.GetStatValue<float>(SkillStatType.HomingRange)
            };

            // ���� ����ü �߻�
            for (int i = 0; i < projectileCount; i++)
            {
                FireProjectile(projectileData);

                // ����ü ������ ������ �ִٸ� ���
                if (innerInterval > 0 && i < projectileCount - 1)
                {
                    yield return new WaitForSeconds(innerInterval);
                }
            }

            // ���� �߻���� ���
            yield return new WaitForSeconds(shotInterval);
        }
    }

    private void FireProjectile(ProjectileData data)
    {
        // ���⼭ �߻� ������ ������ �߰��ϸ� ������
        Projectile projectile = PoolManager.Instance.Spawn<Projectile>
            (projectilePrefab.gameObject, firePoint.position, firePoint.rotation);
        projectile.Initialize(data);
    }

    // ��ų ��ȭ ��
    public override void LevelUp()
    {
        base.LevelUp();

        Debug.Log($"Level {projectileStats.GetStatValue<int>(SkillStatType.SkillLevel)} Stats:");
        Debug.Log($"Damage: {projectileStats.GetStatValue<float>(SkillStatType.Damage)}");
        Debug.Log($"ProjectileCount: {projectileStats.GetStatValue<float>(SkillStatType.ProjectileCount)}");
        Debug.Log($"IsHoming: {projectileStats.GetStatValue<bool>(SkillStatType.IsHoming)}");
    }

    private void OnDisable()
    {
        StopSkill();
    }
}

// ����ü ������ ����ü (����ü�� �����͸� ���� ����ü�� �Ѱ��ִ� ���)
public struct ProjectileData
{
    public float Damage;
    public float Speed;
    public float Range;
    public int PierceCount;
    public bool IsHoming;
    public float HomingRange;
}