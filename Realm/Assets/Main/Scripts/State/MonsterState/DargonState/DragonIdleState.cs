using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonIdleState : State<Dragon>
{
    private float patrolTime;

    public DragonIdleState(Dragon target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.StopMoving();
        target.nextPatrol();
        if ( target.dragonHp < target.CharacterStats.GetStatValue(StatType.MaxHealth)/2f)
        {

        }
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
            return;
        }
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new DragonTakeDamageState(target));
            return;
        }
        var currentAnimatorState = target.Animator.GetCurrentAnimatorStateInfo(0);
        if (currentAnimatorState.IsName("Idle"))
        {
            if (target.CanAttack(target.Target))
            {
                target.M_StateHandler.TransitionTo(new DragonAttackState(target,target.currentSkill.data.skillID));
                return;
            }
            if (target.Target != null)
            {
                target.M_StateHandler.TransitionTo(new FollowState(target));
                return;
            }
            else if (target.FindPlayer(Mathf.Max(target.CharacterStats.GetStatValue(StatType.AttackRange), 20f)))
            {
                target.M_StateHandler.TransitionTo(new FollowState(target));
                return;
            }
            if (patrolTime > 2f)
            {
                target.M_StateHandler.TransitionTo(new MonsterMoveState(target));
                return;
            }
        }
    }
}

