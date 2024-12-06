using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour
{
    private Player player;
    private SkillController skillController;
    [SerializeField] private SkillTreeSlot skillSlotPrefab;
    [SerializeField] private Transform SkillTreeViewPort;
    [SerializeField] private int availableSkillPoints = 0;

    private Dictionary<SkillID, SkillTreeSlot> skillSlots = new Dictionary<SkillID, SkillTreeSlot>();

    public void Initialize(Player player)
    {
        this.player = player;
        skillController = player.GetComponent<SkillController>();
        skillController.OnSkillLevelChanged += UpdateSkillSlotUI;

        CreateSkillSlots();
    }

    private void CreateSkillSlots()
    {
        foreach (Transform child in SkillTreeViewPort)
        {
            Destroy(child.gameObject);
        }
        skillSlots.Clear();

        foreach (Skill skillPrefab in SkillManager.Instance.allSkillsPrefabs)
        {
            SkillTreeSlot slot = Instantiate(skillSlotPrefab, SkillTreeViewPort);
            slot.Initialize(skillPrefab);
            skillSlots.Add(skillPrefab.data.skillID, slot);
        }
    }

    private void UpdateSkillSlotUI(Skill skill)
    {
        if (skillSlots.TryGetValue(skill.data.skillID, out SkillTreeSlot slot))
        {
            slot.UpdateLevel(skill.skillStat.GetStatValue<int>(SkillStatType.SkillLevel));
        }
    }

    public void OnSkillSlotClicked(Skill skillPrefab)
    {
        if (availableSkillPoints > 0)
        {
            if (skillController.TryLevelUpSkill(skillPrefab))
            {
                availableSkillPoints--;
            }
        }
    }

    public void AddSkillPoints(int amount)
    {
        availableSkillPoints += amount;
    }
}