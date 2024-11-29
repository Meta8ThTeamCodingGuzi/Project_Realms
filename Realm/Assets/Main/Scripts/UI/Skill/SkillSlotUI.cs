using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image skillIcon;
    [SerializeField] private Image cooldownOverlay;
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
            hotkeyText.text = key.ToString();
        }
        else
        {
            skillIcon.enabled = false;
            cooldownOverlay.fillAmount = 0;
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
            cooldownOverlay.fillAmount = cooldownRatio;
            cooldownText.text = Mathf.Ceil(currentSkill.RemainingCooldown).ToString();
        }
        else
        {
            cooldownOverlay.fillAmount = 0;
            cooldownText.text = "";
        }
    }
}