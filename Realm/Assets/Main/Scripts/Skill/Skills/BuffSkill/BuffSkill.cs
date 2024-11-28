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
    //지워야함
    private void Start()
    {
        Initialize();
    }
    //지워야함
    public void onskill()
    {
        UseSkill();
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
        print($"적용전 : {GameManager.Instance.player.CharacterStats.GetStatValue(statType)}");
        StatModifier StatModifier = new StatModifier(value , modType, this,SourceType.Buff);
        GameManager.Instance.player.CharacterStats.AddModifier(statType, StatModifier);
        print($"적용후 : {GameManager.Instance.player.CharacterStats.GetStatValue(statType)}");
        GameManager.Instance.player.UpdateMoveSpeed();       
    }

    protected virtual void PlayerRemoveBuff(StatType statType)
    {
        GameManager.Instance.player.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        GameManager.Instance.player.UpdateMoveSpeed();
        print($"버프 빠짐 : {GameManager.Instance.player.CharacterStats.GetStatValue(statType)}");
    }
    #endregion
}
