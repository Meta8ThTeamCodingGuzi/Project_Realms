using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : State<Monster>
{

    public MonsterAttackState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.Attack(target.targetPlayer);
        target.StopMoving();
        target.M_Animator.SetTrigger("Attack");
    }

    public override void OnExit()
    {
        target.StopAttack();
    }

    public override void OnUpdate()
    {
        var currentAnimatorState = target.M_Animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimatorState.normalizedTime >= 1f)
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
            }
            if (!target.wasAttacked || !target.CanAttack(target.targetPlayer))
            {
                target.M_Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
            }
        }
    }

}
