using UnityEngine;

public class Player : Unit
{
    [System.Serializable]
    public class LevelData
    {
        public float baseExpRequired = 100f;  // 1�������� 2������ ���µ� �ʿ��� ����ġ

        [Tooltip("���� ������ ����ġ ������")]
        public float[] growthRates = new float[] { 1.2f, 1.4f, 1.6f };  // ������ ������

        [Tooltip("�������� ����Ǵ� ����")]
        public int[] levelBreakpoints = new int[] { 10, 30, 50 };  // ���� �б���
    }


    [SerializeField] private LevelData levelData;
    private float totalExp = 0f;  // ���� ����ġ

    protected override void Awake()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();
        if (characterStats == null)
        {
            UnitStats unitStats = gameObject.AddComponent<UnitStats>();
            characterStats = unitStats;
        }
    }

    private void Update()
    {
        MovetoCursor();
    }

    private void MovetoCursor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                if (hit.collider.TryGetComponent<Monster>(out Monster monster))
                {
                    Attack(monster);
                }
                else
                {
                    MoveTo(hit.point);
                }
            }
        }
    }

    #region ���� �ý���
    public float TotalExp => totalExp;
    public float ExpToNextLevel => CalculateRequiredExp(GetCurrentLevel());

    // ���� ���������� ����ġ ���� ���
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

        // ������ üũ
        int oldLevel = CalculateLevelFromExp(oldExp);
        int newLevel = CalculateLevelFromExp(totalExp);

        // �������� �߻��ߴٸ�
        if (newLevel > oldLevel)
        {
            for (int i = oldLevel + 1; i <= newLevel; i++)
            {
                PerformLevelUp(i);
            }
        }
    }

    // ���� ����ġ�� ���� ���
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

    // Ư�� �������� �ʿ��� �� ����ġ ���
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
        // ���� ������ �ش��ϴ� ������ ã��
        float currentGrowthRate = levelData.growthRates[0];  // �⺻ ������

        for (int i = 0; i < levelData.levelBreakpoints.Length; i++)
        {
            if (level > levelData.levelBreakpoints[i])
            {
                currentGrowthRate = levelData.growthRates[Mathf.Min(i + 1, levelData.growthRates.Length - 1)];
            }
        }

        // ������ �ʿ� ����ġ ���
        return levelData.baseExpRequired * Mathf.Pow(currentGrowthRate, level - 1);
    }

    private void PerformLevelUp(int newLevel)
    {
        // ���� ����
        StatModifier levelMod = new StatModifier(1f, StatModifierType.Flat, this);
        characterStats.AddModifier(StatType.Level, levelMod);

        // ������ ����
        if (TryGetComponent<StatPointSystem>(out var statPointSystem))
        {
            statPointSystem.AddStatPoints(5);
        }

        OnLevelUp();
    }

    protected virtual void OnLevelUp()
    {
        Debug.Log($"Level Up! Current Level: {GetCurrentLevel()}");
        // ������ ����Ʈ, ���� �� �߰�
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
