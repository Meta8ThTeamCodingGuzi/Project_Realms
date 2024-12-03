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

    public override void Initialize()
    {
        base.Initialize();
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

        for (int i = 0; i < spawnCount; i++)
        {
            AreaEffectData areaData = new AreaEffectData()
            {
                damage = areaSkillStat.GetStatValue<float>(SkillStatType.Damage),
                duration = areaSkillStat.GetStatValue<float>(SkillStatType.Duration),
                areaScale = areaSkillStat.GetStatValue<float>(SkillStatType.SpawnScale),
            };

            SpawnArea(areaData);

            if (spawnInterval > 0 && i < spawnCount - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private void SetSpawnPoint()
    {
        if (areaSkillStat.GetStatValue<int>(SkillStatType.IsSpawnAtCursor) == 1)
        {
            SpawnAtMouseCursor();
        }
        else if (areaSkillStat.GetStatValue<int>(SkillStatType.IsSpawnAtEnemy) == 1)
        {
            SpawnAtEnemyPosition();
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

            // ĳ���Ϳ� Ŀ�� ������ ������ ����մϴ�
            Vector3 direction = (targetPoint - transform.position).normalized;
            direction.y = 0; // Y�� ȸ���� �ʿ��ϹǷ� y���� 0���� ����

            // ĳ���͸� Ŀ�� �������� ȸ����ŵ�ϴ�
            GameManager.Instance.player.transform.rotation = Quaternion.LookRotation(direction);

            // ��ų ���� ���� Ȯ��
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

    private void SpawnAtEnemyPosition()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, areaSkillStat.GetStatValue<float>(SkillStatType.SpawnRange));
        Transform nearestMonster = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<Monster>(out Monster monster))
            {
                float distance = Vector3.Distance(transform.position, monster.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestMonster = monster.transform;
                }
            }
        }

        if (nearestMonster != null)
        {
            // ���� ����� ���� �������� ĳ���͸� ȸ��
            Vector3 direction = (nearestMonster.position - transform.position).normalized;
            direction.y = 0;
            transform.rotation = Quaternion.LookRotation(direction);
            spawnPoint = nearestMonster.position;
        }
        else
        {
            SpawnAtMouseCursor();
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
        PrintAllStats();
    }
}

public struct AreaEffectData
{
    public float damage;
    public float duration;
    public float areaScale;
}
