using UnityEngine;
using System.Collections;

public class BowSkill : WeaponSkill
{
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private Projectile arrowPrefab;
    private float lastFireTime;

    [Header("Bow Animation")]
    [SerializeField] private float drawDuration = 0.3f;  // 활 당기는 시간
    [SerializeField] private float resetDuration = 0.2f;  // 원래 자세로 돌아가는 시간

    private bool isDrawing = false;
    private Coroutine drawRoutine;

    public override void Initialize(Unit owner)
    {
        base.Initialize(owner);
        UpdateArrowSpawnPoint();
    }


    private void UpdateArrowSpawnPoint()
    {
        if (weaponHolder?.CurrentWeaponObject != null)
        {
            Transform weaponSpawnPoint = weaponHolder.CurrentWeaponObject.transform.Find("ArrowSpawnPoint");
            if (weaponSpawnPoint != null)
            {
                arrowSpawnPoint = weaponSpawnPoint;
            }
            else
            {
                Debug.LogWarning("ArrowSpawnPoint not found on weapon prefab");
            }
        }
    }

    protected override void UseSkill()
    {
        if (Time.time - lastFireTime >= 1f / GetPlayerAttackSpeed() && !isDrawing)
        {
            if (drawRoutine != null)
                StopCoroutine(drawRoutine);
            drawRoutine = StartCoroutine(DrawAndFireRoutine());
        }
    }

    private IEnumerator DrawAndFireRoutine()
    {
        isSkillInProgress = true;

        if (weaponHolder.CurrentIKSetup?.offHandIK != null)
        {
            float elapsedTime = 0f;
            float startWeight = weaponHolder.CurrentIKSetup.offHandIK.weight;

            while (elapsedTime < drawDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / drawDuration;
                weaponHolder.CurrentIKSetup.offHandIK.weight = Mathf.Lerp(startWeight, 0.3f, t);
                yield return null;
            }

            FireArrow();
            lastFireTime = Time.time;

            elapsedTime = 0f;
            startWeight = weaponHolder.CurrentIKSetup.offHandIK.weight;

            while (elapsedTime < resetDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / resetDuration;
                weaponHolder.CurrentIKSetup.offHandIK.weight = Mathf.Lerp(startWeight, 0f, t);
                yield return null;
            }
        }

        isSkillInProgress = false;
        player.Animator.SetTrigger("Idle");
        yield break;
    }

    private void FireArrow()
    {
        if (arrowPrefab != null && arrowSpawnPoint != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 targetPoint = ray.GetPoint(distance);

                Vector3 directionToTarget = (targetPoint - transform.position).normalized;
                directionToTarget.y = 0;
                player.transform.rotation = Quaternion.LookRotation(directionToTarget);

                float distanceToTarget = Vector3.Distance(transform.position, targetPoint);
                if (distanceToTarget > GetAttackRange())
                {
                    targetPoint = transform.position + directionToTarget * GetAttackRange();
                }

                Projectile arrow = PoolManager.Instance.Spawn<Projectile>(
                    arrowPrefab.gameObject,
                    arrowSpawnPoint.position,
                    Quaternion.LookRotation(player.transform.forward));

                if (arrow != null)
                {
                    float totalDamage = GetPlayerDamage();

                    ProjectileData data = new ProjectileData
                    {
                        Damage = totalDamage,
                        Speed = 15f,
                        Range = 15f
                    };

                    arrow.Initialize(data);
                }
            }
        }
    }

}