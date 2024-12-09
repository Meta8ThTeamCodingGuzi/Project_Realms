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
    }

    public override void OnUpdate()
    {
        patrolTime += Time.deltaTime;
        if (!target.IsAlive)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
            return;
        }
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
            return; 
        }
        var currentAnimatorState = target.Animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimatorState.IsName("Idle"))
        {
            Debug.Log($"{this} CanAttack ÁØºñ");
            if (target.CanAttack(target.Target))
            {
                target.M_StateHandler.TransitionTo(new MonsterAttackState(target));
                return;
            }
            if (target.Target !=null)
            {
                target.M_StateHandler.TransitionTo(new FollowState(target));
                return;
            }
            else if (target.FindPlayer(Mathf.Max(target.CharacterStats.GetStatValue(StatType.AttackRange),10f)))
            {
                target.M_StateHandler.TransitionTo(new FollowState(target));
                return;
            }
            if (patrolTime > 2f)
            {
                target.M_StateHandler.TransitionTo(new MonsterMoveState(target));
                return;
            }
            return;
        }
    }

}
