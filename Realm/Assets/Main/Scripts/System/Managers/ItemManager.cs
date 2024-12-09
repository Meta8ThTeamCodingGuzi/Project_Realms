using System.Collections.Generic;
using UnityEngine;
using static ItemGenerationRuleSO;

public class ItemManager : SingletonManager<ItemManager>
{
    [SerializeField] private ItemGenerationRuleSO itemGenerationRules;
    [SerializeField] private List<ItemData> itemDataTemplates;
    [SerializeField] private List<ItemData> defaultPlayerItems;

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

        if (itemTemplate.WorldDropPrefab == null)
        {
            Debug.LogError($"WorldDropPrefab is missing for item: {itemTemplate.ItemID}");
            return null;
        }

        GameObject itemObj = Instantiate(itemTemplate.WorldDropPrefab, position, Quaternion.identity);
        Item item = itemObj.GetComponent<Item>();
        if (item != null)
        {
            var instanceData = new ItemInstanceData(itemTemplate);
            instanceData.SetRarity(raritySettings.rarity, raritySettings.itemNameColor);
            GenerateRandomStats(instanceData, dropRule, raritySettings);

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

    public void GiveDefaultItemsToPlayer()
    {
        if (defaultPlayerItems == null || defaultPlayerItems.Count == 0)
        {
            Debug.LogWarning("기본 아이템 목록이 비어있습니다.");
            return;
        }

        var playerInventory = GameManager.Instance.player.InventorySystem;
        if (playerInventory == null)
        {
            Debug.LogError("플레이어 인벤토리를 찾을 수 없습니다.");
            return;
        }

        foreach (var itemTemplate in defaultPlayerItems)
        {
            GameObject itemObj = new GameObject(itemTemplate.ItemID.ToString());
            Item item = itemObj.AddComponent<Item>();

            var instanceData = new ItemInstanceData(itemTemplate);
            instanceData.SetRarity(ItemRarity.Common, Color.white);

            itemInstanceData[item] = instanceData;
            item.Initialize(itemTemplate, instanceData);
            items.Add(item);

            playerInventory.AddItem(item);
        }
    }
}
