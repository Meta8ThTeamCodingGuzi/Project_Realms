using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeSlot : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Skill skillPrefab;  // 이 슬롯에 해당하는 스킬 프리팹

    public SkillID skillID => skillPrefab.data.skillID;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (skillPrefab != null && skillPrefab.data != null)
        {
            skillIcon.sprite = skillPrefab.data.skillIcon;
        }

        levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
        UpdateLevel(0);  // 초기 레벨 표시
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Lv.{level}";
    }

    private void OnLevelUpButtonClicked()
    {
        // SkillTreeUI에서 설정할 이벤트
        GetComponentInParent<SkillTreeUI>()?.OnSkillSlotClicked(skillPrefab);
    }
}