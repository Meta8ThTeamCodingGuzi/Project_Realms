using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : State<Monster>
{
    private float patrolTime;

    public MonsterIdleState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.StopMoving();
        target.nextPatrol();
    }

    public override void OnExit()
    {
        patrolTime = 0;
        base.OnExit();
    }

    public override void OnUpdate()
    {
        patrolTime += Time.deltaTime;
        if (!target.IsAlive)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
        }
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        var currentAnimatorState = target.M_Animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimatorState.IsName("Idle"))
        {
            if (target.FindPlayer(10f))
            {
                target.M_StateHandler.TransitionTo(new FollowState(target));
            }
            if (patrolTime > 5f)
            {
                target.M_StateHandler.TransitionTo(new MonsterMoveState(target));
            }
        }
    }

}
