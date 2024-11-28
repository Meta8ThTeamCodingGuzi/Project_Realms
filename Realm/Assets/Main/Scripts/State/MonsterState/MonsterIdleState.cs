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
        target.nextPatrol();
        base.OnEnter();
        target.M_Animator.SetBool("Idle",true);
    }

    public override void OnExit()
    {
        patrolTime = 0;
        base.OnExit();
        target.M_Animator.SetBool("Idle", false);
    }

    public override void OnUpdate()
    {
        patrolTime += Time.deltaTime;
        if (target.isTakeDamage)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        if (patrolTime > 5f)
        {
            target.M_StateHandler.TransitionTo(new MonsterMoveState(target));
        }
        if (target.FindPlayer(10f))
        {
            target.M_StateHandler.TransitionTo(new FollowState(target));
        }
    }

}
