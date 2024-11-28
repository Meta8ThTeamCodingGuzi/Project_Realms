using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffSkill : Skill
{
    private BuffSkillStat buffSkillStat;
    private Coroutine BuffCoroutine;
    [SerializeField]protected StatType statType;
    [SerializeField]protected StatModifierType modifierType;
    private bool isBuffaction = true;
    private bool isSkillActive = false;

    public override void Initialize()
    {
        base.Initialize();
        buffSkillStat = (BuffSkillStat)skillStat;
        buffSkillStat.InitializeStats();

    }

    protected override void UseSkill()
    {
        if (BuffCoroutine != null)
        {
           StopBuff();
        }
        BuffCoroutine = StartCoroutine(ApplyBuff());
    }
    protected virtual void StopSkill()
    {

    }

    protected virtual void StopBuff()
    {
        if (BuffCoroutine != null)
        {
            StopCoroutine(BuffCoroutine);
            BuffCoroutine = null;
        }
        PlayerRemoveBuff(statType);
    }

    protected virtual IEnumerator ApplyBuff()
    {
        PlayerSetBuff(statType, buffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
        yield return new WaitForSeconds(buffSkillStat.GetStatValue<float>(SkillStatType.Duration));
        PlayerRemoveBuff(statType);
        BuffCoroutine = null;
        yield break;
    }



    private void OnDisable()
    {
        StopBuff();
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    #region 플레이어 버프세팅 버프제거 로직
    protected virtual void PlayerSetBuff(StatType statType,float value,StatModifierType modType) 
    {
        StatModifier StatModifier = new StatModifier(value , modType, this,SourceType.Buff);
        GameManager.Instance.player.CharacterStats.AddModifier(statType, StatModifier);
        GameManager.Instance.player.UpdateMoveSpeed();
    }

    protected virtual void PlayerRemoveBuff(StatType statType)
    {
        GameManager.Instance.player.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        GameManager.Instance.player.UpdateMoveSpeed();
    }
    #endregion
}
