using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class AreaSkill : Skill
{
    [SerializeField]private FakeArea areaPrefab;
    [SerializeField]private Vector3 spawnPoint;
    private AreaSkillStat areaSkillStat;
    private Coroutine spawnCoroutine;
    private bool isSpawnPoint = false;
    private bool isSkillActive = false;


    public override void Initialize()
    {
        base.Initialize();
        areaSkillStat = (AreaSkillStat)skillStat;
        areaSkillStat.InitializeStats();
    }

    //임시로 만든거임 삭제해야함
    public void skillshot()
    {
        Initialize();
        UseSkill();
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
        if (true)//areaSkillStat.GetStatValue<bool>(SkillStatType.IsTraceMouse))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out RaycastHit hit, 1000f);

            spawnPoint = hit.point;

            if (Vector3.Distance(transform.position, hit.point)>areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange))
            {
                Vector3 dir = (hit.point - transform.position).normalized;
                spawnPoint = dir * areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange);
            }

            isSpawnPoint = false;
            
        }
        spawnPoint = this.transform.position;
        isSpawnPoint = true;
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
            float ShotInterval = areaSkillStat.GetStatValue<float>(SkillStatType.Cooldown);

            AreaData areaData = new AreaData()
            {
                damage = areaSkillStat.GetStatValue<float>(SkillStatType.Damage),
                duration =areaSkillStat.GetStatValue<float>(SkillStatType.Duration),
                areaScale = areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileScale),
            };

            SpawnArea(areaData);

         yield return new WaitForSeconds(ShotInterval);

        }
    }

    private void SpawnArea(AreaData data)
    {
        FakeArea area = PoolManager.Instance.Spawn<FakeArea>
        (areaPrefab.gameObject, spawnPoint, Quaternion.identity);
        if (isSpawnPoint) area.transform.SetParent(this.transform); 
        area.Initialize(data);
    }


    public override void LevelUp()
    {
        base.LevelUp();
    }

 
}

public struct AreaData
{
    public float damage;
    public float duration;
    public float areaScale;

}
