using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DebuffSKill : Skill
{
    [SerializeField]private Debuff DebuffPrefab;
    private Debuff DebuffParticle;
    private DeBuffSkillStat deBuffSkillStat;
    private bool isSkillActive = false;
    private bool clickDelay = true;


    public override void Initialize(Unit owner)
    {
        base.Initialize(owner);
        deBuffSkillStat = (DeBuffSkillStat)skillStat;
        deBuffSkillStat.InitializeStats();
        this.transform.localScale = Vector3.one * deBuffSkillStat.GetStatValue<float>(SkillStatType.DeBuffAreaScale);
    }

    protected override void UseSkill()
    {
        Owner.Animator.SetTrigger("Attack");
        if (clickDelay == false) return;
        if (!isSkillActive)
        {
            DebuffParticle = PoolManager.Instance.Spawn<Debuff>(DebuffPrefab.gameObject,Owner.transform.position,Quaternion.Euler(90,0,0));
            DebuffParticle.transform.SetParent(Owner.transform);
            DebuffParticle.transform.localPosition = Vector3.zero;
            DebuffParticle.Initialize(Owner, deBuffSkillStat);
            isSkillActive = true;
        }
        else if (isSkillActive)
        {
            PoolManager.Instance.Despawn<Debuff>(DebuffParticle);
            isSkillActive = false;
        }
        Owner.Animator.SetTrigger("Idle");
        StartCoroutine(ClickRoutine());
    }
    private IEnumerator ClickRoutine()
    {
        clickDelay = false;
        yield return new WaitForSeconds(0.1f);
        clickDelay = true;
    }


    
}
