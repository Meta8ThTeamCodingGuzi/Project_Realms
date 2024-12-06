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
        target.StopMoving();
        MoveStateTime = 0f;
    }

    public override void OnExit()
    {
        MoveStateTime = 0;
        target.Animator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {
        MoveStateTime += Time.deltaTime;
        if (target is Dragon dragon)
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new DragonTakeDamageState(dragon));
            }
            if (target.FindPlayer(15f))
            {
                target.M_StateHandler.TransitionTo(new FollowState(dragon));
            }
            if (target.HasReachedDestination() || MoveStateTime > 15f)
            {
                target.Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new DragonIdleState(dragon));
            }

        }
        else
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
            }
            if (target.FindPlayer(6f))
            {
                target.M_StateHandler.TransitionTo(new FollowState(target));
                return;
            }
            if (target.HasReachedDestination() || MoveStateTime > 15f)
            {
                target.Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
            }
        }
        if (!target.IsMoving && target.Target == null && !target.CanAttack(target.Target))
        {
            target.Animator.SetBool("Move", true);
            target.MoveTo(target.currentPatrolPoint);
        }
    }
}