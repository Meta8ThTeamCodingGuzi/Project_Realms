using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;

public class AreaSkill : Skill
{
    [SerializeField] private AreaEffect areaPrefab;
    [SerializeField] private Vector3 spawnPoint;
    private AreaSkillStat areaSkillStat;

    public override void Initialize(Unit owner)
    {
        base.Initialize(owner);
        areaSkillStat = (AreaSkillStat)skillStat;
        areaSkillStat.InitializeStats();
    }


    protected override void UseSkill()
    {
        StartCoroutine(SpawnSequence());
    }

    private IEnumerator SpawnSequence()
    {
        int spawnCount = Mathf.RoundToInt(areaSkillStat.GetStatValue<float>(SkillStatType.SpawnCount));
        float spawnInterval = areaSkillStat.GetStatValue<float>(SkillStatType.SpawnInterval);

        isSkillInProgress = true;
        
        for (int i = 0; i < spawnCount; i++)
        {
            AreaEffectData areaData = new AreaEffectData()
            {
                owner = Owner,
                damage = areaSkillStat.GetStatValue<float>(SkillStatType.Damage),
                areaScale = areaSkillStat.GetStatValue<float>(SkillStatType.SpawnScale),
                duration = areaSkillStat.GetStatValue<float>(SkillStatType.Duration)
            };

            SpawnArea(areaData);

            if (spawnInterval > 0 && i < spawnCount - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        
        Owner.Animator.SetTrigger("Idle");

        yield return new WaitForSeconds(0.3f);
        isSkillInProgress = false;
    }

    private void SetSpawnPoint()
    {
        if (areaSkillStat.GetStatValue<int>(SkillStatType.IsSpawnAtCursor) == 1)
        {
            SpawnAtMouseCursor();
        }
        else if (areaSkillStat.GetStatValue<int>(SkillStatType.IsSpawnAtEnemy) == 1)
        {
            SpawnAtTargetPosition();
        }
        else
        {
            SpawnAtMouseCursor();
        }
    }

    private void SpawnAtMouseCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);

            Vector3 direction = (targetPoint - transform.position).normalized;
            direction.y = 0;

            Owner.transform.rotation = Quaternion.LookRotation(direction);

            if (Vector3.Distance(transform.position, targetPoint) > areaSkillStat.GetStatValue<float>(SkillStatType.SpawnRange))
            {
                spawnPoint = transform.position + direction * areaSkillStat.GetStatValue<float>(SkillStatType.SpawnRange);
            }
            else
            {
                spawnPoint = targetPoint;
            }
        }
    }

    private void SpawnAtTargetPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, areaSkillStat.GetStatValue<float>(SkillStatType.SpawnRange));
        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            if(Owner.TryGetComponent<Player>(out Player player)) 
            {
                if (collider.TryGetComponent<Monster>(out Monster Monster))
                {
                    float distance = Vector3.Distance(transform.position, Monster.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestTarget = Monster.transform;
                    }
                }
                else
                {
                    SpawnAtMouseCursor();
                }
            }
            if (Owner.TryGetComponent<Monster>(out Monster monster)) 
            {
                if (collider.TryGetComponent<Player>(out Player Player))
                {
                    float distance = Vector3.Distance(transform.position, Player.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestTarget = Player.transform;
                    }
                }
            }

        }
        if (nearestTarget != null)
        {
            Vector3 direction = (nearestTarget.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            spawnPoint = nearestTarget.position;
        }
    }


    private void SpawnArea(AreaEffectData data)
    {
        SetSpawnPoint();
        AreaEffect area = PoolManager.Instance.Spawn<AreaEffect>
            (areaPrefab.gameObject, spawnPoint, Quaternion.identity);
        area.Initialize(data);
    }


    public override void LevelUp()
    {
        base.LevelUp();
    }
}

public struct AreaEffectData
{
    public Unit owner;
    public float damage;
    public float duration;
    public float areaScale;
}
