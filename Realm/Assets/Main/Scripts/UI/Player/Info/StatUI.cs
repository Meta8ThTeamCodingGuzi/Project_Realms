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
            // 버튼에 리스너 추가
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
        // 사용 가능한 포인트 표시
        if (availablePointsText != null)
        {
            availablePointsText.text = $"{statPointSystem.AvailablePoints}";
        }

        // 각 스탯 표시 업데이트
        foreach (var display in statDisplays)
        {
            float value = unitStats.GetStatValue(display.statType);
            display.valueText.text = value.ToString("F1");

            // 버튼 활성화/비활성화
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