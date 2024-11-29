using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private TextMeshProUGUI hotkeyText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    private Skill currentSkill;
    private KeyCode hotkey;

    public void SetSkill(Skill skill, KeyCode key)
    {
        currentSkill = skill;
        hotkey = key;

        if (skill != null && skill.data != null)
        {
            skillIcon.sprite = skill.data.skillIcon.sprite;
            skillIcon.enabled = true;
            skillIcon.color = Color.white;
            hotkeyText.text = key.ToString();
        }
        else
        {
            skillIcon.enabled = false;
            hotkeyText.text = key.ToString();
            cooldownText.text = "";
        }
    }

    private void Update()
    {
        if (currentSkill == null) return;

        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        if (currentSkill.IsOnCooldown)
        {
            float cooldownRatio = currentSkill.RemainingCooldown / currentSkill.TotalCooldown;
            Color iconColor = Color.Lerp(Color.gray, Color.white, 1 - cooldownRatio);
            skillIcon.color = iconColor;
            cooldownText.text = Mathf.Ceil(currentSkill.RemainingCooldown).ToString();
        }
        else
        {
            skillIcon.color = Color.white;
            cooldownText.text = "";
        }
    }
}