using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Tooltip("이 클래스는 ISHoming을 켜야 정상작동 합니다.")]
public class PointAttackSkill : Skill
{
    [SerializeField] private PointAttack PointAttackPrefab;
    [SerializeField] private Vector3 spawnPoint;
    private AreaSkillStat areaSkillStat;
    private Coroutine spawnCoroutine;
    private bool isSkillActive = false;

    //임시로 만든거임 삭제해야함
    public void skillshot()
    {
        Initialize();
        UseSkill();
    }

    public override void Initialize()
    {
        base.Initialize();
        areaSkillStat = (AreaSkillStat)skillStat;
        areaSkillStat.InitializeStats();
    }

    protected override void UseSkill()
    {
        if (!isSkillActive)
        {
            isSkillActive = true;
            spawnCoroutine = StartCoroutine(SpawnRoutine());
        }
    }
    protected virtual void StopSkill()
    {
        if (isSkillActive)
        {
            isSkillActive = false;
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }
    private void SetSpawnPoint()
    {
        if (true)//areaSkillStat.GetStatValue<bool>(SkillStatType.IsHoming))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position,
                areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange));
            {
                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent<Monster>(out Monster monster))
                    {
                        spawnPoint = monster.transform.position;
                        return;
                    }
                }
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit hit, 1000f);

        spawnPoint = hit.point;

        if (Vector3.Distance(transform.position, hit.point) > areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange))
        {
            Vector3 dir = (hit.point - transform.position).normalized;
            spawnPoint = dir * areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange);
        }



    }

    private void OnDisable()
    {
        StopSkill();
    }

    private IEnumerator SpawnRoutine()
    {
        while (isSkillActive)
        {

            float areaRange = areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange);
            float coolDown = areaSkillStat.GetStatValue<float>(SkillStatType.Cooldown);
            float innerInterval = areaSkillStat.GetStatValue<float>(SkillStatType.InnerInterval);
            float pointAttackCount = Mathf.RoundToInt(areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileCount));


            PointData pointData = new PointData()
            {
                damage = areaSkillStat.GetStatValue<float>(SkillStatType.Damage),
                duration = areaSkillStat.GetStatValue<float>(SkillStatType.Duration),
                Scale = areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileScale),
                Range = areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange),
            };

            for (int i = 0; i < pointAttackCount; i++)
            {
                SetSpawnPoint();
                SpawnPointAttack(pointData);

                if (innerInterval > 0 && i < pointAttackCount - 1)
                {
                    yield return new WaitForSeconds(innerInterval);
                }
            }

            yield return new WaitForSeconds(coolDown);

        }
    }

    private void SpawnPointAttack(PointData data)
    {
        PointAttack area = PoolManager.Instance.Spawn<PointAttack>
            (PointAttackPrefab.gameObject, spawnPoint, Quaternion.identity);
        area.Initialize(data);
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }


}

public struct PointData
{
    public float damage;
    public float duration;
    public float Scale;
    public float Range;

}

