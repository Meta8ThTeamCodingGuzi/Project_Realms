using UnityEngine;
using System.Collections.Generic;

public class SkillBarUI : MonoBehaviour
{
    [System.Serializable]
    public struct SkillSlotMapping
    {
        public KeyCode hotkey;
        public SkillSlotUI slotUI;
    }

    [SerializeField] private SkillController skillController;
    [SerializeField] private List<SkillSlotMapping> skillSlots;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (skillController == null)
        {
            skillController = FindObjectOfType<SkillController>();
        }

        InitializeSlots();
    }

    private void InitializeSlots()
    {
        foreach (var slot in skillSlots)
        {
            slot.slotUI.SetSkill(null, slot.hotkey);
        }
    }

    public void UpdateSkillSlot(KeyCode hotkey, Skill skill)
    {
        var slot = skillSlots.Find(s => s.hotkey == hotkey);
        if (slot.slotUI != null)
        {
            slot.slotUI.SetSkill(skill, hotkey);
        }
    }
}