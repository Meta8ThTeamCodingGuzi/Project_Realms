using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Player player;
    public OrbUI mpOrb;
    public OrbUI hpOrb;
    public Slider playerExp;
    public TextMeshProUGUI playerLevelText;
    public StatUI statUI;

    public void Initialize(Player player)
    {
        this.player = player;
        statUI = GetComponentInChildren<StatUI>();
        statUI.Initialize(player);
        statUI.gameObject.SetActive(false);
    }

    public void ShowStatUI()
    {
        statUI.gameObject.SetActive(true);
        if (statUI != null)
            statUI.UpdateUI();
    }

    public void HideStatUI()
    {
        statUI.gameObject.SetActive(false);
    }

    public void UpdatePlayerInfo()
    {
        float maxHealth = player.CharacterStats.GetStatValue(StatType.MaxHealth);
        float maxMana = player.CharacterStats.GetStatValue(StatType.MaxMana);

        float hp = maxHealth > 0 ? player.CharacterStats.GetStatValue(StatType.Health) / maxHealth : 0f;
        float mp = maxMana > 0 ? player.CharacterStats.GetStatValue(StatType.Mana) / maxMana : 0f;

        hp = Mathf.Clamp01(hp);
        mp = Mathf.Clamp01(mp);

        UpdateOrbs(mp, hp);

        float expAmount = player.ExpPercentage;

        int level = (int)player.CharacterStats.GetStatValue(StatType.Level);

        UpdatePlayerLevel(level);

        UpdatePlayerEXP(expAmount);
    }

    public void UpdatePlayerLevel(int level)
    {
        playerLevelText.text = level.ToString();
    }

    public void UpdateOrbs(float manaAmount, float hpAmount)
    {
        hpOrb.ChangeOrbValue(hpAmount);
        mpOrb.ChangeOrbValue(manaAmount);
    }

    public void UpdatePlayerEXP(float amount)
    {
        playerExp.value = amount;
    }
}
