using UnityEngine;
using System.Collections.Generic;

public class SkillController : MonoBehaviour
{
    private const int MAX_ACTIVE_SKILLS = 6; // QWER ���� ��

    [SerializeField] private Player player;
    private Dictionary<KeyCode, Skill> skillSlots = new Dictionary<KeyCode, Skill>();
    public List<Skill> availableSkills = new List<Skill>();
    public List<Skill> activeSkills = new List<Skill>();
    [SerializeField] private SkillBarUI skillBarUI;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (player == null)
            player = GetComponent<Player>();

        skillSlots[KeyCode.Q] = null;
        skillSlots[KeyCode.W] = null;
        skillSlots[KeyCode.E] = null;
        skillSlots[KeyCode.R] = null;
        skillSlots[KeyCode.Space] = null;
        skillSlots[KeyCode.Mouse0] = null;
    }

    private void Update()
    {
        CheckSkillInputs();
    }

    private void CheckSkillInputs()
    {
        foreach (var slot in skillSlots)
        {
            if (Input.GetKey(slot.Key) && slot.Value != null)
            {
                slot.Value.TryUseSkill();
            }
        }
    }

    public void AddSkill(Skill skill)
    {
        if (!availableSkills.Contains(skill))
        {
            availableSkills.Add(skill);
            skill.Initialize();
        }
    }

    public void EquipSkill(Skill skill, KeyCode slot)
    {
        if (!skillSlots.ContainsKey(slot))
            return;

        if (!availableSkills.Contains(skill))
            return;

        if (skillSlots[slot] != null)
        {
            activeSkills.Remove(skillSlots[slot]);
        }

        skillSlots[slot] = skill;
        if (!activeSkills.Contains(skill))
        {
            activeSkills.Add(skill);
        }

        if (skillBarUI != null)
        {
            skillBarUI.UpdateSkillSlot(slot, skill);
        }
    }

    public void UnequipSkill(KeyCode slot)
    {
        if (skillSlots.ContainsKey(slot) && skillSlots[slot] != null)
        {
            activeSkills.Remove(skillSlots[slot]);
            skillSlots[slot] = null;

            if (skillBarUI != null)
            {
                skillBarUI.UpdateSkillSlot(slot, null);
            }
        }
    }
}