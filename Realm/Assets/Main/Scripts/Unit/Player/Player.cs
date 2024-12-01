﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Unit
{
    [System.Serializable]
    public class LevelData
    {
        public float baseExpRequired = 100f;  // 1레벨에서 2레벨로 가는데 필요한 경험치

        [Tooltip("레벨 구간별 경험치 증가율")]
        public float[] growthRates = new float[] { 1.2f, 1.4f, 1.6f };  // 구간별 증가율

        [Tooltip("증가율이 변경되는 레벨")]
        public int[] levelBreakpoints = new int[] { 10, 30, 50 };  // 구간 분기점
    }

    [SerializeField] private LevelData levelData;

    internal SkillController skillController;

    private float totalExp = 0f;  // 누적 경험치

    private LayerMask groundLayerMask;

    private StatPointSystem statPoint;

    private PlayerHandler playerHandler;
    public PlayerHandler PlayerHandler => playerHandler;

    private Animator playerAnimator;
    public Animator PlayerAnimator => playerAnimator;

    private PlayerAnimatorController animaControl;
    public PlayerAnimatorController AnimaControl => animaControl;

    public Playerjob playerjob = Playerjob.knight;


    private void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        Debug.Log("Player Initialize 시작");

        if (characterStats == null)
        {
            characterStats = GetComponent<PlayerStat>();

            if (characterStats == null)
            {
                Debug.LogError("PlayerStat component not found!");
                return;
            }
        }

        groundLayerMask = LayerMask.GetMask("Ground");

        statPoint = GetComponent<StatPointSystem>();

        statPoint.Initialize();

        GameManager.Instance.player = this;

        skillController = GetComponent<SkillController>();

        if (skillController == null)
        {
            skillController = gameObject.AddComponent<SkillController>();
        }


        skillController.Initialize();

        base.Initialize();
        if (playerHandler == null)
        {
            playerHandler = new PlayerHandler(this);
            playerHandler.Initialize();
        }
        Debug.Log("Player initialized successfully");
        playerAnimator = GetComponent<Animator>();

        animaControl = GetComponent<PlayerAnimatorController>();
        if (animaControl == null)
        {
            animaControl = gameObject.AddComponent<PlayerAnimatorController>();
        }
    }

    private void Update()
    {
        playerHandler.HandleUpdate();
        MovetoCursor();
    }

    private void MovetoCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI 요소 클릭 체크
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return; // UI 요소를 클릭한 경우 이동하지 않음
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Debug.Log($"Hit object layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                if (hit.collider.TryGetComponent<Monster>(out Monster monster))
                {
                    Attack(monster);
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, 1000f, groundLayerMask))
            {
                //Debug.Log($"Ground hit at: {hit.point}");

                MoveTo(hit.point);
            }
            else
            {
                //Debug.Log("No ground detected");
            }
        }
    }
    public void PlayerDie()
    {
        StartCoroutine(DieCroutine());
    }

    public IEnumerator DieCroutine()
    {
        playerAnimator.SetTrigger("Die");
        yield return new WaitForSeconds(3f);
        PoolManager.Instance.Despawn(this);
    }
    public void PlayerAnimatorChange(RuntimeAnimatorController newController)
    {
        playerAnimator.runtimeAnimatorController = newController;
    }

    #region 레벨 시스템
    public float TotalExp => totalExp;

    public float ExpToNextLevel => CalculateRequiredExp(GetCurrentLevel());

    // 현재 레벨에서의 경험치 비율 계산
    public float ExpPercentage
    {
        get
        {
            int currentLevel = GetCurrentLevel();

            float expForCurrentLevel = CalculateTotalExpForLevel(currentLevel);

            float expForNextLevel = CalculateTotalExpForLevel(currentLevel + 1);

            return (totalExp - expForCurrentLevel) / (expForNextLevel - expForCurrentLevel);
        }
    }

    public void GainExperience(float amount)
    {
        if (!IsAlive) return;

        float oldExp = totalExp;

        totalExp += amount;

        // 레벨업 체크
        int oldLevel = CalculateLevelFromExp(oldExp);

        int newLevel = CalculateLevelFromExp(totalExp);

        // 레벨업이 발생했다면
        if (newLevel > oldLevel)
        {
            for (int i = oldLevel + 1; i <= newLevel; i++)
            {
                PerformLevelUp(i);
            }
        }
    }

    // 누적 경험치로 레벨 계산
    private int CalculateLevelFromExp(float exp)
    {
        int level = 1;

        float nextLevelExp = CalculateTotalExpForLevel(level + 1);

        while (exp >= nextLevelExp)
        {
            level++;

            nextLevelExp = CalculateTotalExpForLevel(level + 1);
        }

        return level;
    }

    // 특정 레벨까지 필요한 총 경험치 계산
    private float CalculateTotalExpForLevel(int level)
    {
        float total = 0f;

        for (int i = 1; i < level; i++)
        {
            total += CalculateRequiredExp(i);
        }

        return total;
    }

    private float CalculateRequiredExp(int level)
    {
        // 현재 레벨에 해당하는 증가율 찾기
        float currentGrowthRate = levelData.growthRates[0];  // 기본 증가율

        for (int i = 0; i < levelData.levelBreakpoints.Length; i++)
        {
            if (level > levelData.levelBreakpoints[i])
            {
                currentGrowthRate = levelData.growthRates[Mathf.Min(i + 1, levelData.growthRates.Length - 1)];
            }
        }

        // 레벨별 필요 경험치 계산
        return levelData.baseExpRequired * Mathf.Pow(currentGrowthRate, level - 1);
    }

    private void PerformLevelUp(int newLevel)
    {
        // 레벨 증가
        StatModifier levelMod = new StatModifier(1f, StatModifierType.Flat, this);

        characterStats.AddModifier(StatType.Level, levelMod);

        // 레벨업 보상
        statPoint.AddStatPoints(5);

        OnLevelUp();
    }

    protected virtual void OnLevelUp()
    {
        Debug.Log($"Level Up! Current Level: {GetCurrentLevel()}");

        // 레벨업 이펙트, 사운드 등 추가
    }

    private int GetCurrentLevel()
    {
        return Mathf.RoundToInt(characterStats.GetStatValue(StatType.Level));
    }
    #endregion

    public float GetAttack()
    {
        return characterStats.GetStatValue(StatType.Damage);
    }

    public float GetDefense()
    {
        return characterStats.GetStatValue(StatType.Defense);
    }
}
