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
        if (projectileStats == null)
        {
            Debug.LogError($"{gameObject.name}: projectileStats가 null입니다!");
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
            Debug.LogError($"{gameObject.name}: projectilePrefab이 없습니다!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError($"{gameObject.name}: firePoint가 없습니다!");
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

            // 풀에서 프로젝타일을 가져오기 전에 프리팹이 유효한지 한번 더 확인
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

                        // Despawn 전에 projectile이 아직 유효한지 확인
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
                    Debug.LogError($"{gameObject.name}: 프로젝타일 생성 중 오류 발생: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"{gameObject.name}: 프로젝타일 프리팹이 유효하지 않습니다!");
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