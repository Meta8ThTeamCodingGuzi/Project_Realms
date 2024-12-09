using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuffSkill : Skill
{
    [SerializeField] private Buff BuffPrefab;
    private Buff buffParticle;
    private BuffSkillStat buffSkillStat;
    private Coroutine BuffCoroutine;
    [SerializeField]protected StatType statType;
    [SerializeField]protected StatModifierType modifierType;

    public override void Initialize(Unit owner)
    {
        base.Initialize(owner);
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
    protected virtual void StopSkill(){}

    protected virtual void StopBuff()
    {
        if (BuffCoroutine != null)
        {
            StopCoroutine(BuffCoroutine);
            BuffCoroutine = null;
        }
        RemoveBuff(statType);

    }

    protected virtual IEnumerator ApplyBuff()
    {
        Owner.Animator.SetTrigger("Attack");
        buffParticle = PoolManager.Instance.Spawn<Buff>(BuffPrefab.gameObject, Owner.transform.position, Quaternion.Euler(90, 0, 0));
        buffParticle.transform.SetParent(Owner.transform);
        buffParticle.transform.localPosition = Vector3.zero;
        SetBuff(statType, buffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
        Owner.Animator.SetTrigger("Idle");
        yield return new WaitForSeconds(buffSkillStat.GetStatValue<float>(SkillStatType.Duration));
        RemoveBuff(statType);
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

    #region �÷��̾� �������� �������� ����
    protected virtual void SetBuff(StatType statType,float value,StatModifierType modType) 
    {
        print($"������ : {Owner.CharacterStats.GetStatValue(statType)}");
        StatModifier StatModifier = new StatModifier(value , modType, this,SourceType.Buff);
        Owner.CharacterStats.AddModifier(statType, StatModifier);
        print($"������ : {Owner.CharacterStats.GetStatValue(statType)}");
        Owner.UpdateMoveSpeed();       
    }

    protected virtual void RemoveBuff(StatType statType)
    {
        if (buffParticle != null)
        {
            PoolManager.Instance.Despawn(buffParticle);
        }
        Owner.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        Owner.UpdateMoveSpeed();
        print($"���� ���� : {Owner.CharacterStats.GetStatValue(statType)}");
    }
    #endregion
}
