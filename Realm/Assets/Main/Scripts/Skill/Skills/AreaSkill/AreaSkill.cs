using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class AreaSkill : Skill
{
    [SerializeField]private Area areaPrefab;
    [SerializeField]private Vector3 spawnPoint;
    private AreaSkillStat areaSkillStat;
    private Coroutine spawnCoroutine;
    private bool isSkillActive = false;


    private void Awake()
    {
        areaSkillStat = (AreaSkillStat)skillStat;
        
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
        if (areaSkillStat.GetStatValue<bool>(SkillStatType.IsTraceMouse))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Physics.Raycast(ray, out RaycastHit hit, 1000f);

            spawnPoint = hit.point;

            if (Vector3.Distance(transform.position, hit.point)>areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange))
            {
                Vector3 dir = (hit.point - transform.position).normalized;
                spawnPoint = dir * areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange);
            }
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
            SetSpawnPoint();
            float areaRange = areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileRange);
            float ShotInterval = areaSkillStat.GetStatValue<float>(SkillStatType.Cooldown);

            AreaData areaData = new AreaData()
            {
                Damage = areaSkillStat.GetStatValue<float>(SkillStatType.Damage),
                Duration =areaSkillStat.GetStatValue<float>(SkillStatType.Duration),
                AreaScale = areaSkillStat.GetStatValue<float>(SkillStatType.ProjectileScale),
                
            };

            SpawnArea(areaData);

         yield return new WaitForSeconds(ShotInterval);

        }
    }

    private void SpawnArea(AreaData data)
    {
        Area area = PoolManager.Instance.Spawn<Area>
            (areaPrefab.gameObject, spawnPoint,Quaternion.identity);
        area.Initialize(data);
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

 
}

public struct AreaData
{
    public float Damage;
    public float Duration;
    public float AreaScale;

}
