using System.Collections.Generic;
using UnityEngine;
using static ItemGenerationRuleSO;

public class ItemManager : SingletonManager<ItemManager>
{
    [SerializeField] private ItemGenerationRuleSO itemGenerationRules;
    [SerializeField] private List<ItemData> itemDataTemplates;

    private List<Item> items = new List<Item>();
    private Dictionary<Item, ItemInstanceData> itemInstanceData = new Dictionary<Item, ItemInstanceData>();

    public Item GenerateRandomItem(MonsterType monsterType, Vector3 position)
    {
        var dropRule = itemGenerationRules.GetDropRuleForMonster(monsterType);
        if (dropRule == null) return null;

        if (UnityEngine.Random.Range(0f, 100f) > dropRule.itemDropChance)
            return null;

        var raritySettings = itemGenerationRules.GetRaritySettings(dropRule);
        ItemType randomItemType = dropRule.possibleItemTypes[UnityEngine.Random.Range(0, dropRule.possibleItemTypes.Length)];

        var itemTemplate = itemDataTemplates.Find(t => t.ItemType == randomItemType);
        if (itemTemplate == null) return null;

        // 월드에 아이템 생성
        if (itemTemplate.WorldDropPrefab == null)
        {
            Debug.LogError($"WorldDropPrefab is missing for item: {itemTemplate.ItemID}");
            return null;
        }

        GameObject itemObj = Instantiate(itemTemplate.WorldDropPrefab, position, Quaternion.identity);
        Item item = itemObj.GetComponent<Item>();
        if (item != null)
        {
            // 아이템 인스턴스 데이터 생성
            var instanceData = new ItemInstanceData(itemTemplate);
            instanceData.SetRarity(raritySettings.rarity, raritySettings.itemNameColor);
            GenerateRandomStats(instanceData, dropRule, raritySettings);

            // 아이템과 인스턴스 데이터 연결
            itemInstanceData[item] = instanceData;
            item.Initialize(itemTemplate, instanceData);
            items.Add(item);
        }

        return item;
    }

    private void GenerateRandomStats(ItemInstanceData itemInstance, MonsterDropRule dropRule, ItemGenerationRuleSO.RaritySettings raritySettings)
    {
        List<StatType> availableStats = new List<StatType>(dropRule.possibleStatTypes);
        ShuffleList(availableStats);

        int statCount = Mathf.Min(
            UnityEngine.Random.Range(raritySettings.statCountRange.x, raritySettings.statCountRange.y + 1),
            availableStats.Count
        );

        for (int i = 0; i < statCount; i++)
        {
            StatType selectedStatType = availableStats[i];
            float randomValue = UnityEngine.Random.Range(raritySettings.statValueRange.x, raritySettings.statValueRange.y);

            float rarityMultiplier = 1f + ((int)raritySettings.rarity * 0.2f);
            randomValue *= rarityMultiplier;

            itemInstance.AddStat(selectedStatType, randomValue, 0);
        }        
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void RemoveItem(Item item)
    {
        if (itemInstanceData.ContainsKey(item))
        {
            itemInstanceData.Remove(item);
        }
        items.Remove(item);
    }

    public ItemInstanceData GetItemInstanceData(Item item)
    {
        if (itemInstanceData.TryGetValue(item, out ItemInstanceData instance))
        {
            return instance;
        }
        return null;
    }
}
