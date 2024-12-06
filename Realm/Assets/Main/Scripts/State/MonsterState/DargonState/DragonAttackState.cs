using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttackState : State<Dragon>
{
    private SkillID skillId;

    public DragonAttackState(Dragon target, SkillID skillID) : base(target)
    {
        this.target = target;
        this.skillId = skillID;
    }

    public override void OnEnter()
    {
        target.StopMoving();
        target.GetSkill(skillId).TryUseSkill();
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        var currentAnimatorState = target.Animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimatorState.normalizedTime >= 1f)
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new DragonTakeDamageState(target));
            }
            if (!target.wasAttacked || !target.CanAttack(target.Target))
            {
                target.Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new DragonIdleState(target));
            }
            target.Animator.SetTrigger("Idle");
        }
    }
}
