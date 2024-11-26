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
    }

    public override void OnExit()
    {
        patrolTime = 0;
    }

    public override void OnUpdate()
    {
        patrolTime = Time.deltaTime;
        if (target.CharacterStats.GetStatValue(StatType.Health) <= 0)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
        }
        if (patrolTime > 3f)
        {
            target.M_StateHandler.TransitionTo(new MonsterMoveState(target));
        }
        if (target.FindPlayer(10f))
        {
            target.M_StateHandler.TransitionTo(new FollowState(target));
        }
    }

}
