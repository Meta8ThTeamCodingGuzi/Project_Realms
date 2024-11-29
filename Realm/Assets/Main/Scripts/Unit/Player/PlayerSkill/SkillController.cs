using UnityEngine;
using System.Collections.Generic;

public class SkillController : MonoBehaviour
{
    private const int MAX_ACTIVE_SKILLS = 6;

    [SerializeField] private Player player;
    private Dictionary<KeyCode, Skill> skillSlots = new Dictionary<KeyCode, Skill>();
    public List<Skill> availableSkills = new List<Skill>();
    public List<Skill> activeSkills = new List<Skill>();
    [SerializeField] private SkillBarUI skillBarUI;

    private void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        foreach (var skill in availableSkills)
        {
            if (skill != null)
            {
                Destroy(skill.gameObject);
            }
        }
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
        }
    }

    public void EquipSkill(Skill skillPrefab, KeyCode slot)
    {
        if (!skillSlots.ContainsKey(slot))
            return;

        if (!availableSkills.Contains(skillPrefab))
            return;

        if (skillSlots[slot] != null)
        {
            Destroy(skillSlots[slot].gameObject);
            activeSkills.Remove(skillSlots[slot]);
        }

        Skill newSkill = Instantiate(skillPrefab, transform);
        newSkill.Initialize();

        skillSlots[slot] = newSkill;
        activeSkills.Add(newSkill);

        if (skillBarUI != null)
        {
            skillBarUI.UpdateSkillSlot(slot, newSkill);
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