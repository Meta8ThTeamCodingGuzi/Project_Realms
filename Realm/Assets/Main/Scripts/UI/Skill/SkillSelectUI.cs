using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject skillSelectPanel;
    [SerializeField] private Transform skillButtonContainer;
    [SerializeField] private SkillSelectButton skillButtonPrefab;
    [SerializeField] private SkillController skillController;
    [SerializeField] private Vector2 offset = new Vector2(0, 10f);
    private RectTransform panelRectTransform;

    private KeyCode currentSelectedSlot;
    private List<SkillSelectButton> spawnedButtons = new List<SkillSelectButton>();

    private void Start()
    {
        Initialize();
        panelRectTransform = skillSelectPanel.GetComponent<RectTransform>();
    }

    private void Initialize()
    {
        if (skillController == null)
            skillController = FindObjectOfType<SkillController>();

        skillSelectPanel.SetActive(false);
    }

    public void ShowSkillSelect(KeyCode slotKey, RectTransform slotTransform)
    {
        if (skillSelectPanel.activeSelf && currentSelectedSlot != slotKey)
        {
            ClosePanel();
        }

        currentSelectedSlot = slotKey;
        ClearSkillButtons();
        CreateSkillButtons();

        Vector2 slotPosition = slotTransform.position;
        panelRectTransform.position = slotPosition + offset;

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
        foreach (Skill skillPrefab in skillController.availableSkillPrefabs)
        {
            Skill initializedSkill = skillController.GetInitializedSkill(skillPrefab.data.skillID);

            SkillSelectButton button = Instantiate(skillButtonPrefab, skillButtonContainer);
            button.Initialize(initializedSkill, OnSkillSelected);
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