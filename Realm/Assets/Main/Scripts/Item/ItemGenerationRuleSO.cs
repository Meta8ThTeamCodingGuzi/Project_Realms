using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item Generation Rule", menuName = "Game/Item Generation Rule")]
public class ItemGenerationRuleSO : ScriptableObject
{
    // �⺻ ��� ���� ����
    private static readonly Color[] DefaultRarityColors = new Color[]
    {
        Color.white,                              // Common
        new Color(0.118f, 1f, 0f),               // Uncommon (�ʷ�)
        new Color(0f, 0.439f, 0.867f),           // Rare (�Ķ�)
        new Color(0.639f, 0.208f, 0.933f),       // Epic (����)
        new Color(1f, 0.502f, 0f)                // Legendary (��Ȳ)
    };

    [Serializable]
    public class RaritySettings
    {
        public ItemRarity rarity;
        [Range(0f, 100f)]
        public float dropChance;
        public Vector2Int statCountRange;
        public Vector2 statValueRange;
        public Color itemNameColor = Color.white;

        public void SetDefaultColor()
        {
            itemNameColor = DefaultRarityColors[(int)rarity];
        }
    }

    [Serializable]
    public class MonsterDropRule
    {
        public MonsterType monsterType;
        [Range(0f, 100f)]
        public float itemDropChance;
        public ItemType[] possibleItemTypes;
        public StatType[] possibleStatTypes;
        public RaritySettings[] raritySettings;
    }

    public MonsterDropRule[] monsterDropRules;

    public MonsterDropRule GetDropRuleForMonster(MonsterType monsterType)
    {
        return Array.Find(monsterDropRules, rule => rule.monsterType == monsterType);
    }

    public RaritySettings GetRaritySettings(MonsterDropRule rule)
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float accumulatedChance = 0f;

        foreach (var settings in rule.raritySettings)
        {
            accumulatedChance += settings.dropChance;
            if (randomValue <= accumulatedChance)
            {
                return settings;
            }
        }

        return rule.raritySettings[0]; // �⺻������ ù ��° ��� ��ȯ
    }

    private void OnValidate()
    {
        // �����Ϳ��� ��� ���� �� �⺻ ���� �ڵ� ����
        foreach (var rule in monsterDropRules)
        {
            if (rule.raritySettings != null)
            {
                foreach (var settings in rule.raritySettings)
                {
                    if (settings.itemNameColor == Color.white)
                    {
                        settings.SetDefaultColor();
                    }
                }
            }
        }
    }
}