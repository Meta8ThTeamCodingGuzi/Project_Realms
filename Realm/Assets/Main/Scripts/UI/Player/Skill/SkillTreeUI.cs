using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour
{
    [SerializeField] private SkillController skillController;
    [SerializeField] private List<SkillTreeSlot> skillSlots;  // �� ��ų�� UI ����
    [SerializeField] private int availableSkillPoints = 0;

    private void Start()
    {
        skillController.OnSkillLevelChanged += UpdateSkillSlotUI;
    }

    private void UpdateSkillSlotUI(Skill skill)
    {
        // �ش� ��ų�� UI ���� ã��
        var slot = skillSlots.Find(s => s.skillID == skill.data.skillID);
        if (slot != null)
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