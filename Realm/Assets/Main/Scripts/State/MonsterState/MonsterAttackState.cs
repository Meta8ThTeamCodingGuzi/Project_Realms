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
        target.M_Animator.SetTrigger("Attack");
        target.Attack(target.targetPlayer);
        base.OnEnter();
    }

    public override void OnExit()
    {
        target.StopAttack();
        base.OnExit();
    }

    public override void OnUpdate()
    {
        if (target.isTakeDamage)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        if (!target.FindPlayer(10f))
        {
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
        if (!target.CanAttack(target.targetPlayer))
        {
            target.M_StateHandler.TransitionTo(new FollowState(target));
        }


    }

}
