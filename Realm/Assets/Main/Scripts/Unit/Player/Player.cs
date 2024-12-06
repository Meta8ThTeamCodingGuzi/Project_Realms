using System.Collections;
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


    private PlayerStateHandler playerHandler;
    public PlayerStateHandler PlayerHandler => playerHandler;



    private Vector3 targetPos = Vector3.zero;
    public Vector3 TargetPos => targetPos;

    private PlayerInventorySystem inventorySystem;
    public PlayerInventorySystem InventorySystem => inventorySystem;

    [Header("Regeneration Settings")]
    [SerializeField] private float regenTickTime = 1f;      // 리젠 틱 간격
    private Coroutine healthRegenCoroutine;
    private Coroutine manaRegenCoroutine;

    // groundLayerMask에 대한 getter 추가
    public LayerMask GroundLayer => groundLayerMask;

    private PlayerInputManager inputManager;
    public PlayerInputManager InputManager => inputManager;

    private void Start()
    {
        Initialize();
    }

    public override void Initialize()
    {

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

        statPoint.Initialize(this);

        GameManager.Instance.player = this;

        skillController = GetComponent<SkillController>();

        if (skillController == null)
        {
            skillController = gameObject.AddComponent<SkillController>();
        }

        skillController.Initialize();

        inventorySystem = GetComponent<PlayerInventorySystem>();

        if (inventorySystem == null)
        {
            inventorySystem = gameObject.AddComponent<PlayerInventorySystem>();
        }

        AnimController = GetComponent<AnimatorController>();

        if (AnimController == null)
        {
            AnimController = gameObject.AddComponent<AnimatorController>();
        }
        AnimController.Initialize(this);

        Animator = GetComponent<Animator>();

        if (Animator == null)
        {
            Animator = gameObject.AddComponent<Animator>();
        }

        playerHandler = new PlayerStateHandler(this);
        playerHandler.Initialize();

        base.Initialize();

        // 리젠 코루틴 시작
        StartRegeneration();

        inputManager = gameObject.AddComponent<PlayerInputManager>();
        inputManager.Initialize(this);

        Debug.Log("Player initialized successfully");
    }

    private void Update()
    {
        playerHandler.HandleUpdate();
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

    private void StartRegeneration()
    {
        // 기존 코루틴이 실행 중이라면 중지
        if (healthRegenCoroutine != null)
            StopCoroutine(healthRegenCoroutine);
        if (manaRegenCoroutine != null)
            StopCoroutine(manaRegenCoroutine);

        // 새로운 코루틴 시작
        healthRegenCoroutine = StartCoroutine(HealthRegenCoroutine());
        manaRegenCoroutine = StartCoroutine(ManaRegenCoroutine());
    }

    private IEnumerator HealthRegenCoroutine()
    {
        while (IsAlive)
        {
            float currentHealth = characterStats.GetStatValue(StatType.Health);
            float maxHealth = characterStats.GetStatValue(StatType.MaxHealth);
            float regenRate = characterStats.GetStatValue(StatType.HealthRegenRate);

            if (currentHealth < maxHealth)
            {
                characterStats.AddModifier(StatType.Health, new StatModifier(regenRate, StatModifierType.Flat, SourceType.BaseStats));
            }

            yield return new WaitForSeconds(regenTickTime);
        }
    }

    private IEnumerator ManaRegenCoroutine()
    {
        while (IsAlive)
        {
            float currentMana = characterStats.GetStatValue(StatType.Mana);
            float maxMana = characterStats.GetStatValue(StatType.MaxMana);
            float regenRate = characterStats.GetStatValue(StatType.ManaRegenRate);

            if (currentMana < maxMana)
            {
                characterStats.AddModifier(StatType.Mana, new StatModifier(regenRate, StatModifierType.Flat, SourceType.BaseStats));
            }

            yield return new WaitForSeconds(regenTickTime);
        }
    }
    public void PlayerDie()
    {
        StartCoroutine(DieRoutine());
    }

    public IEnumerator DieRoutine()
    {
        Animator.SetTrigger("Die");

        while (!Animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
        {
            yield return null;
        }

        while (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        //플레이어 죽고나서 해야할거 해야할듯
    }
    public void ClearTarget()
    {
        Target = null;
    }


    private void OnDisable()
    {
        // 코루틴 정리
        if (healthRegenCoroutine != null)
            StopCoroutine(healthRegenCoroutine);
        if (manaRegenCoroutine != null)
            StopCoroutine(manaRegenCoroutine);
    }

    public void SetTarget(Unit monster)
    {
        Target = monster;
        targetPos = Vector3.zero;
    }

    public void SetDestination(Vector3 position)
    {
        targetPos = position;
        Target = null;
    }

    public override void MoveTo(Vector3 destination)
    {
        if (Target != null)
        {
            Vector3 directionToTarget = (destination - transform.position).normalized;
            float attackRange = characterStats.GetStatValue(StatType.AttackRange);

            Vector3 targetPosition = destination - (directionToTarget * attackRange);
            base.MoveTo(targetPosition);
        }
        else
        {
            base.MoveTo(destination);
        }
    }
}
