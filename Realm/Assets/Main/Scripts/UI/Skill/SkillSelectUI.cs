using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject skillSelectPanel;
    [SerializeField] private Transform skillButtonContainer;
    [SerializeField] private SkillSelectButton skillButtonPrefab;
    [SerializeField] private SkillController skillController;

    private KeyCode currentSelectedSlot;
    private List<SkillSelectButton> spawnedButtons = new List<SkillSelectButton>();

    private void Start()
    {
        if (skillController == null)
            skillController = FindObjectOfType<SkillController>();

        skillSelectPanel.SetActive(false);
    }

    public void ShowSkillSelect(KeyCode slotKey)
    {
        currentSelectedSlot = slotKey;
        ClearSkillButtons();
        CreateSkillButtons();
        skillSelectPanel.SetActive(true);
    }

    private void ClearSkillButtons()
    {
        foreach (var button in spawnedButtons)
        {
            Destroy(button.gameObject);
        }
        spawnedButtons.Clear();
    }

    private void CreateSkillButtons()
    {
        foreach (Skill skill in skillController.availableSkills)
        {
            SkillSelectButton button = Instantiate(skillButtonPrefab, skillButtonContainer);
            button.Initialize(skill, OnSkillSelected);
            spawnedButtons.Add(button);
        }
    }

    private void OnSkillSelected(Skill selectedSkill)
    {
        skillController.EquipSkill(selectedSkill, currentSelectedSlot);
        skillSelectPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        skillSelectPanel.SetActive(false);
    }
}