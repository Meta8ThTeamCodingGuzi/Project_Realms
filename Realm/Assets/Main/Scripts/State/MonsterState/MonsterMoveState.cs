using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
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
        Debug.Log("��������Ʈ");
        target.StopMoving();
        MoveStateTime = target.UpdateTime;
    }

    public override void OnExit()
    {

        target.Animator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {
        Debug.Log($"{target.IsMoving}");
        MoveStateTime += Time.deltaTime;

        if (!target.IsMoving)
        {
            target.Animator.SetBool("Move", true);
            target.MoveTo(target.currentPatrolPoint);
        }
        if (target is Dragon dragon)
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new DragonTakeDamageState(dragon));
                return;
            }
            if (target.FindPlayer(15f))
            {
                target.M_StateHandler.TransitionTo(new FollowState(dragon));
                return;
            }
            if (target.UpdateTime-MoveStateTime > 5f)
            {
                target.Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new DragonIdleState(dragon));
                return;
            }

        }
        else
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
                return;
            }
            if (target.FindPlayer(6f))
            {
                target.M_StateHandler.TransitionTo(new FollowState(target));
                return;
            }
            if (target.UpdateTime-MoveStateTime > 5f)
            {
                target.Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
                return;
            }
        }
    }
}