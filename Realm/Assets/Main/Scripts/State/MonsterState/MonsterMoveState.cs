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
        target.Animator.SetBool("Move", true);
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
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        if (target.FindPlayer(6f))
        {
            Debug.Log($"타킷  : {target.Target} , 몬스터 팔로우스테이트 진입 ");
            target.M_StateHandler.TransitionTo(new FollowState(target));
            return;
        }
        if (!target.IsMoving &&target.Target ==null &&!target.CanAttack(target.Target))
        { 
            target.MoveTo(target.currentPatrolPoint);
        }
        if (target.HasReachedDestination()|| MoveStateTime>15f)
        {
            target.Animator.SetTrigger("Idle");
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
    }
}
