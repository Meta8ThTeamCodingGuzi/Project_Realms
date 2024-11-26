using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveState : State<Monster>
{
    public MonsterMoveState(Monster target) : base(target)
    {
        this.target = target;
    }
    private float MoveStateTime = 0;

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        MoveStateTime = 0;
        base.OnExit();
    }

    public override void OnUpdate()
    {
        MoveStateTime += Time.deltaTime;
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
        if (target.HasReachedDestination()|| MoveStateTime>15f)
        {
            target.StopMoving();
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
    }
}
