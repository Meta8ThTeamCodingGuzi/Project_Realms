using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    [Serializable]
    public class ItemStat
    {
        public StatType statType;
        public float flatValue;
        public float percentValue;

        public string GetTooltipText()
        {
            string text = "";
            if (flatValue != 0)
                text += $"{statType} +{flatValue}\n";
            if (percentValue != 0)
                text += $"{statType} +{percentValue}%\n";
            return text;
        }
    }

    [SerializeField] private ItemID itemID;
    [SerializeField] private ItemType itemType;
    [SerializeField] private string description = "";
    [SerializeField] private Sprite icon;
    [SerializeField] private List<ItemStat> stats = new List<ItemStat>();
    [SerializeField] private GameObject itemPrefab;

    [Header("Weapon Skill")]
    [SerializeField] private Skill defaultWeaponSkill; // 무기의 기본 스킬
    public Skill DefaultWeaponSkill => defaultWeaponSkill;

    public ItemID ItemID => itemID;
    public ItemType ItemType => itemType;
    public string Description => description;
    public Sprite Icon => icon;
    public IReadOnlyList<ItemStat> Stats => stats;
    public GameObject ItemPrefab => itemPrefab;

    public string GetTooltip()
    {
        string tooltip = $"[{itemType}] {itemID}\n{description}\n\n";
        foreach (var stat in stats)
        {
            tooltip += stat.GetTooltipText();
        }
        return tooltip.TrimEnd('\n');
    }

    // 무기 타입별 기본 스킬 프리팹 가져오기
    public Skill GetDefaultSkillForWeapon()
    {
        if (itemType != ItemType.Sword && itemType != ItemType.Bow)
            return null;

        return defaultWeaponSkill;
    }
}