using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class StatUI : MonoBehaviour
{
    [System.Serializable]
    public class StatDisplay
    {
        public StatType statType;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI valueText;
        public Button increaseButton;
    }

    [SerializeField] private StatDisplay[] statDisplays;
    [SerializeField] private TextMeshProUGUI availablePointsText;

    private Player player;
    private UnitStats unitStats;
    private StatPointSystem statPointSystem;

    public void Initialize(Player player)
    {
        this.player = player;
        unitStats = player.GetComponent<UnitStats>();
        statPointSystem = player.GetComponent<StatPointSystem>();

        InitializeUI();
    }

    private void InitializeUI()
    {
        foreach (var display in statDisplays)
        {
            // ��ư�� ������ �߰�
            if (display.increaseButton != null)
            {
                StatType statType = display.statType;
                display.increaseButton.onClick.AddListener(() => OnStatIncreaseButton(statType));
            }
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        // ��� ������ ����Ʈ ǥ��
        if (availablePointsText != null)
        {
            availablePointsText.text = $"{statPointSystem.AvailablePoints}";
        }

        // �� ���� ǥ�� ������Ʈ
        foreach (var display in statDisplays)
        {
            float value = unitStats.GetStatValue(display.statType);
            display.valueText.text = value.ToString("F1");

            // ��ư Ȱ��ȭ/��Ȱ��ȭ
            if (display.increaseButton != null)
            {
                bool canInvest = statPointSystem.AvailablePoints > 0 &&
                                unitStats.GetPointIncreaseAmount(display.statType) > 0;
                display.increaseButton.interactable = canInvest;
            }
        }
    }

    private void OnStatIncreaseButton(StatType statType)
    {
        if (statPointSystem.TryInvestPoint(statType))
        {
            UpdateUI();
        }
    }
}