using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : State<Monster>
{
    public FollowState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        target.M_Animator.SetBool("Move", true);
    }

    public override void OnExit()
    {
        base.OnExit();
        target.M_Animator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {
        if (!target.IsAlive)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
        }
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        if (target.CanAttack(target.targetPlayer))
        {
            target.M_StateHandler.TransitionTo(new MonsterAttackState(target));
        }
        if (!target.FindPlayer(10f))
        {
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
        target.targetMove(target.targetPlayer);
    }
}
