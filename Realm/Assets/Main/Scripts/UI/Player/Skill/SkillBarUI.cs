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
    [SerializeField] private GameObject skillSelectUIPrefab;
     
    public void Initialize(Player player)
    {
        skillController = GameManager.Instance.player.skillController;  
        
        skillController.SetSkillBarUI(this);

        SkillSelectUI skillSelectUI = Instantiate(skillSelectUIPrefab,transform).GetComponent<SkillSelectUI>();

        InitializeSlots(skillSelectUI);
    }

    private void InitializeSlots(SkillSelectUI ssu)
    {
        foreach (var slot in skillSlots)
        {
            slot.slotUI.SetSkillSelectUI(ssu);
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