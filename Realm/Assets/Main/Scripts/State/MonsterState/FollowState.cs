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
        target.StopMoving();
    }

    public override void OnExit()
    {
        target.M_Animator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        if (target.CanAttack(target.Target))
        {
            target.M_StateHandler.TransitionTo(new MonsterAttackState(target));
        }
        if (!target.FindPlayer(Mathf.Max(target.CharacterStats.GetStatValue(StatType.AttackRange), 10f)))
        {
            target.M_Animator.SetTrigger("Idle");
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
        if (target.Target != null && !target.CanAttack(target.Target))
        {
            if (target.M_Animator.GetBool("Move") != true) { target.M_Animator.SetBool("Move", true); }
            target.targetMove(target.Target);
        }
    }
}
