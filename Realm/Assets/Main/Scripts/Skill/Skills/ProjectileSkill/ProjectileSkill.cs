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
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError($"{gameObject.name}: projectilePrefab 또는 firePoint가 없습니다!");
            return;
        }

        // 마우스 커서의 월드 좌표를 구합니다
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);

            // 캐릭터와 커서 사이의 방향을 계산합니다
            Vector3 direction = (targetPoint - transform.position).normalized;
            direction.y = 0; // Y축 회전만 필요하므로 y값은 0으로 설정

            GameManager.Instance.player.transform.rotation = Quaternion.LookRotation(direction);

            // firePoint도 같은 방향을 바라보도록 설정합니다
            firePoint.rotation = transform.rotation;

            // 발사체를 생성하고 초기화합니다
            Projectile projectile = PoolManager.Instance.Spawn<Projectile>
                (projectilePrefab.gameObject, firePoint.position, firePoint.rotation);

            if (projectile != null)
            {
                projectile.Initialize(data);
                PoolManager.Instance.Despawn<Projectile>(projectile, skillStat.GetStatValue<float>(SkillStatType.Duration));
            }
            else
            {
                Debug.LogError($"{gameObject.name}: 발사체 생성 실패!");
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