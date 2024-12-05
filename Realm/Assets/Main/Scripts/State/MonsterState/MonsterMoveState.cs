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
        target.M_Animator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {
        MoveStateTime += Time.deltaTime;
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        if (target.FindPlayer(Mathf.Max(target.CharacterStats.GetStatValue(StatType.AttackRange),6f)))
        {
            target.M_StateHandler.TransitionTo(new FollowState(target));
            return;
        }
        if (!target.IsMoving &&target.targetPlayer ==null &&!target.CanAttack(target.targetPlayer))
        {
            if (target.M_Animator.GetBool("Move") != true) { target.M_Animator.SetBool("Move", true); }
            target.MoveTo(target.nowTarget);
        }
        if (target.HasReachedDestination()|| MoveStateTime>10f)
        {
            target.M_Animator.SetTrigger("Idle");
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
    }
}
