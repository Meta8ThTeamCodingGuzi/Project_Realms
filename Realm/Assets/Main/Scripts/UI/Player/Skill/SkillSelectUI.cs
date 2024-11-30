using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillSelectUI : MonoBehaviour
{
    public GameObject skillSelectPanel;
    public Transform skillButtonContainer;
    public SkillSelectButton skillButtonPrefab;
    public Vector2 offset = new Vector2(0, 10f);
    public RectTransform panelRectTransform;

    private KeyCode currentSelectedSlot;
    private List<SkillSelectButton> spawnedButtons = new List<SkillSelectButton>();

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (panelRectTransform == null)
        {
            panelRectTransform = skillSelectPanel.GetComponent<RectTransform>();
        }
        if (skillSelectPanel != null)
        {
            skillSelectPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("skillSelectPanel is not assigned!");
        }
    }

    public void ShowSkillSelect(KeyCode slotKey, RectTransform slotTransform)
    {
        if (skillSelectPanel == null) return;

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
        if (spawnedButtons == null) return;

        foreach (var button in spawnedButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        spawnedButtons.Clear();
    }

    private void CreateSkillButtons()
    {
        foreach (Skill skillPrefab in GameManager.Instance.player.skillController.availableSkillPrefabs)
        {
            Skill initializedSkill = GameManager.Instance.player.skillController.GetInitializedSkill(skillPrefab.data.skillID);

            SkillSelectButton button = Instantiate(skillButtonPrefab, skillButtonContainer);
            button.Initialize(initializedSkill, OnSkillSelected);
            spawnedButtons.Add(button);
        }
    }

    private void OnSkillSelected(Skill selectedSkill)
    {
        GameManager.Instance.player.skillController.EquipSkill(selectedSkill, currentSelectedSlot);
        skillSelectPanel.SetActive(false);
    }

    public void ClosePanel()
    {
        skillSelectPanel.SetActive(false);
    }
}