using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillTreeSlot : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button levelUpButton;
    [SerializeField] private Skill skillPrefab;  // �� ���Կ� �ش��ϴ� ��ų ������

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
        UpdateLevel(0);  // �ʱ� ���� ǥ��
    }

    public void UpdateLevel(int level)
    {
        levelText.text = $"Lv.{level}";
    }

    private void OnLevelUpButtonClicked()
    {
        // SkillTreeUI���� ������ �̺�Ʈ
        GetComponentInParent<SkillTreeUI>()?.OnSkillSlotClicked(skillPrefab);
    }
}