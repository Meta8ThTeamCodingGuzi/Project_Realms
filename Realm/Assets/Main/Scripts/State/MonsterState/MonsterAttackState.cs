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
    }

    public override void OnExit()
    {
        target.StopAttack();
    }

    public override void OnUpdate()
    {
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
