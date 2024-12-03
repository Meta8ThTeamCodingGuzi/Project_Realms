using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Item Generation Rule", menuName = "Game/Item Generation Rule")]
public class ItemGenerationRuleSO : ScriptableObject
{
    // 기본 레어도 색상 정의
    private static readonly Color[] DefaultRarityColors = new Color[]
    {
        Color.white,                              // Common
        new Color(0.118f, 1f, 0f),               // Uncommon (초록)
        new Color(0f, 0.439f, 0.867f),           // Rare (파랑)
        new Color(0.639f, 0.208f, 0.933f),       // Epic (보라)
        new Color(1f, 0.502f, 0f)                // Legendary (주황)
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

        return rule.raritySettings[0]; // 기본값으로 첫 번째 레어도 반환
    }

    private void OnValidate()
    {
        // 에디터에서 레어도 설정 시 기본 색상 자동 적용
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