using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class AreaSkill : Skill
{
    [SerializeField]private Area areaPrefab;
    [SerializeField] private Transform firePoint;
    private AreaSkillStat areaSkillStat;
    private Coroutine fireCoroutine;
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

    private void OnDisable()
    {
        StopSkill();
    }

    private IEnumerator FireRoutine()
    {
        while (isSkillActive)
        {




        }
        yield return null;
    }

    private void FireArea(AreaData data)
    {
        Area area = PoolManager.Instance.Spawn<Area>
            (areaPrefab.gameObject, firePoint.position,Quaternion.identity);
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
    public float Speed;
    public float Range;
    public int PierceCount;

}
