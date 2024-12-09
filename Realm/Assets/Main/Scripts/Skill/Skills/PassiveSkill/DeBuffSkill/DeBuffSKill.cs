using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DebuffSKill : Skill
{
    [SerializeField]private Debuff DebuffPrefab;
    private Debuff DebuffParticle;
    private DeBuffSkillStat deBuffSkillStat;
    private List<Monster> monsters = new List<Monster>();
    [SerializeField] protected StatType statType;
    [SerializeField] protected StatModifierType modifierType;
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

    private void Update()
    {
        //OnDebuff();
    }

    private void OnDebuff()
    {
        if (!isSkillActive) return;
        Collider[] colliders = Physics.OverlapSphere(Owner.transform.position, deBuffSkillStat.GetStatValue<float>(SkillStatType.DeBuffAreaScale));

        if (Owner.TryGetComponent<Monster>(out Monster monster))
        {
            foreach (var collider in colliders)
            {

                if (collider.TryGetComponent<Player>(out Player player))
                {
                    if (player != null)
                    {
                        SetDeBuff(player, statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
                        print("몬스터가 플레이어에게 디버프 적용");
                    }
                }
            }
        }
        if (Owner.TryGetComponent<Player>(out Player Player))
        {
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<Monster>(out Monster Monster))
                {
                    if (monster != null && !monsters.Contains(monster))
                    {
                        monsters.Add(monster);
                        SetDeBuff(monster, statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
                        print("플레이어가 몬스터에게 디버프 적용");
                    }
                }
            }
        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isSkillActive || other == null) return;
    //    if (Owner.TryGetComponent<Monster>(out Monster monster)) 
    //    {
    //        if (other.TryGetComponent<Player>(out Player player)) 
    //        {
    //            if (player != null) 
    //            {
    //                SetDeBuff(player,statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
    //                print("몬스터가 플레이어에게 디버프 적용");
    //            }
    //        }
    //    }
    //    if (Owner.TryGetComponent<Player>(out Player Player))
    //    {
    //        if (other.TryGetComponent<Monster>(out Monster Monster)) 
    //        {
    //            if (monster != null && !monsters.Contains(monster))
    //            {
    //                monsters.Add(monster);
    //                SetDeBuff(monster, statType, deBuffSkillStat.GetStatValue<float>(SkillStatType.BuffValue), modifierType);
    //                print("플레이어가 몬스터에게 디버프 적용");
    //            }
    //        }
    //    }

    //}
    private void OnTriggerExit(Collider other)
    {
        if (Owner.TryGetComponent<Monster>(out Monster monster))
        {
            if (other.TryGetComponent<Player>(out Player player))
            {
                if (player != null)
                {
                    RemoveDeBuff(player, statType);
                    print("플레이어 디버프 해제");
                }
            }
        }
        if (Owner.TryGetComponent<Player>(out Player Player))
        {
            if (other != null && other.TryGetComponent<Monster>(out Monster Monster))
            {
                if (Monster != null && monsters.Contains(Monster))
                {
                    RemoveDeBuff(Monster, statType);
                    monsters.Remove(Monster);
                }
            }
        }
    }


    protected virtual void OnDisable()
    {
        if (DebuffParticle != null)
        {
            PoolManager.Instance.Despawn<Debuff>(DebuffParticle);
        }

        foreach (Monster monster in monsters)
        {
            RemoveDeBuff(monster, statType);
        }
        monsters.Clear();
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }

    #region 버프적용 버프제거 로직
    protected virtual void SetDeBuff(Unit target, StatType statType, float value, StatModifierType modType)
    {
        StatModifier statModifier = new StatModifier(value, modType, this, SourceType.Debuff);
        target.CharacterStats.AddModifier(statType, statModifier);
        target.UpdateMoveSpeed();
    }

    protected virtual void RemoveDeBuff(Unit target, StatType statType)
    {
        target.CharacterStats.GetStat(statType)?.RemoveAllModifiersFromSource(this);
        target.UpdateMoveSpeed();
        print("디버프 해제");
    }
    #endregion

}
