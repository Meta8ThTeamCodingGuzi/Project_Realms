using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveState : State<Monster>
{
    public MonsterMoveState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        if (!target.IsAlive)
        {
            
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
            return;
        }
        if (target.FindPlayer(6f))
        {
            target.M_StateHandler.TransitionTo(new FollowState(target));
            return;
        }
        if (!target.IsMoving) 
        { 
            target.MoveTo(target.nowTarget);
        }
        if (target.HasReachedDestination())
        {
            target.StopMoving();
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
    }
}
