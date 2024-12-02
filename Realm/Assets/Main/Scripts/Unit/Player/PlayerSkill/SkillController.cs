using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class SkillController : MonoBehaviour
{
    private const int MAX_ACTIVE_SKILLS = 6;
    public List<Skill> availableSkillPrefabs = new List<Skill>();
    public List<Skill> activeSkills = new List<Skill>();
    private Player player;
    private Dictionary<KeyCode, Skill> skillSlots = new Dictionary<KeyCode, Skill>();
    private Dictionary<SkillID, Skill> initializedSkills = new Dictionary<SkillID, Skill>();
    private SkillBarUI skillBarUI;
    public KeyCode skillActivated;

    public void Initialize()
    {
        if (player == null)
            player = GetComponent<Player>();

        skillSlots[KeyCode.Q] = null;
        skillSlots[KeyCode.W] = null;
        skillSlots[KeyCode.E] = null;
        skillSlots[KeyCode.R] = null;
        skillSlots[KeyCode.Mouse0] = null;
        skillSlots[KeyCode.Space] = null;

        foreach (var skillPrefab in availableSkillPrefabs)
        {
            Skill instance = Instantiate(skillPrefab, transform);
            instance.Initialize();
            initializedSkills[skillPrefab.data.skillID] = instance;
        }
    }

    public void SetSkillBarUI(SkillBarUI skillBarUI)
    {
        this.skillBarUI = skillBarUI;
    }

    //private void Update()
    //{
    //    CheckSkillInputs();
    //}

    public bool CheckSkillInputs()
    {
        bool anySkillUsed = false;

        foreach (var slot in skillSlots)
        {
            if (Input.GetKeyDown(slot.Key) && slot.Value != null)
            {
                if (slot.Key == KeyCode.Mouse0)
                {
                    continue;
                }

                if (slot.Value.TryUseSkill())
                {
                    anySkillUsed = true;
                }
            }
        }
        return anySkillUsed;
    }

    public void OnMouseClick()
    {
        if (Input.GetMouseButtonDown(0) && skillSlots.ContainsKey(KeyCode.Mouse0))
        {
            if (skillSlots[KeyCode.Mouse0] != null)
            {
                skillSlots[KeyCode.Mouse0].TryUseSkill();
            }
        }
    }

    public void AddSkill(Skill skill)
    {
        if (!availableSkillPrefabs.Contains(skill))
        {
            Skill instance = Instantiate(skill, transform);
            instance.Initialize();
            availableSkillPrefabs.Add(instance);
            initializedSkills[skill.data.skillID] = instance;
        }
    }

    public bool IsSkillEquipped(Skill skillPrefab)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.Value != null && slot.Value.data.skillID == skillPrefab.data.skillID)
            {
                return true;
            }
        }
        return false;
    }

    public void EquipSkill(Skill skillPrefab, KeyCode newSlot)
    {
        if (!skillSlots.ContainsKey(newSlot))
            return;

        KeyCode existingSlot = KeyCode.None;
        foreach (var slot in skillSlots)
        {
            if (slot.Value != null && slot.Value.data.skillID == skillPrefab.data.skillID)
            {
                existingSlot = slot.Key;
                break;
            }
        }

        if (existingSlot != KeyCode.None)
        {
            skillSlots[existingSlot] = null;
            if (skillBarUI != null)
            {
                skillBarUI.UpdateSkillSlot(existingSlot, null);
            }
        }

        if (skillSlots[newSlot] != null)
        {
            skillSlots[newSlot] = null;
        }

        Skill skillInstance = initializedSkills[skillPrefab.data.skillID];
        skillSlots[newSlot] = skillInstance;

        if (!activeSkills.Contains(skillInstance))
        {
            activeSkills.Add(skillInstance);
        }

        if (skillBarUI != null)
        {
            skillBarUI.UpdateSkillSlot(newSlot, skillInstance);
        }
    }

    public void UnequipSkill(KeyCode slot)
    {
        if (skillSlots.ContainsKey(slot) && skillSlots[slot] != null)
        {
            Skill skillToRemove = skillSlots[slot];
            activeSkills.Remove(skillToRemove);
            skillSlots[slot] = null;

            if (skillBarUI != null)
            {
                skillBarUI.UpdateSkillSlot(slot, null);
            }
        }
    }

    public Skill GetInitializedSkill(SkillID skillID)
    {
        if (initializedSkills.TryGetValue(skillID, out Skill skill))
        {
            return skill;
        }
        return null;
    }

    public bool TryLevelUpSkill(Skill skillPrefab)
    {
        if (!initializedSkills.TryGetValue(skillPrefab.data.skillID, out Skill existingSkill))
        {
            AddSkill(skillPrefab);
            existingSkill = initializedSkills[skillPrefab.data.skillID];
        }

        existingSkill.SetLevel(skillPrefab.skillStat.GetStatValue<int>(SkillStatType.SkillLevel) + 1);

        OnSkillLevelChanged?.Invoke(existingSkill);

        return true;
    }

    public int GetSkillLevel(SkillID skillID)
    {
        if (initializedSkills.TryGetValue(skillID, out Skill skill))
        {
            return skill.skillStat.GetStatValue<int>(SkillStatType.SkillLevel);
        }
        return 0;
    }

    public event Action<Skill> OnSkillLevelChanged;

    public void DirectEquipSkill(Skill skillPrefab, KeyCode slot)
    {
        if (!skillSlots.ContainsKey(slot))
            return;

        if (skillSlots[slot] != null)
        {
            UnequipSkill(slot);
        }

        Skill instance = Instantiate(skillPrefab, transform);
        instance.Initialize();

        activeSkills.Add(instance);
        initializedSkills[skillPrefab.data.skillID] = instance;
        skillSlots[slot] = instance;

        if (skillBarUI != null)
        {
            skillBarUI.UpdateSkillSlot(slot, instance);
        }
    }

    // 현재 활성화된 스킬들을 반환하는 메서드
    public IEnumerable<Skill> GetCurrentSkills()
    {
        return activeSkills;
    }

    // 특정 슬롯의 스킬을 가져오는 메서드 (필요한 경우)
    public Skill GetSkillInSlot(KeyCode slot)
    {
        if (skillSlots.TryGetValue(slot, out Skill skill))
        {
            return skill;
        }
        return null;
    }
}