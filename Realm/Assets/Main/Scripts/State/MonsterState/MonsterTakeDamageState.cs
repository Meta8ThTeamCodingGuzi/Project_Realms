using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTakeDamageState : State<Monster>
{
    public MonsterTakeDamageState(Monster target) : base(target)
    {
        this.target = target;
    }

    private float MoveStateTime = 0;

    public override void OnEnter()
    {
        base.OnEnter();
        target.M_Animator.SetTrigger("TakeDamage");
    }

    public override void OnExit()
    {
        MoveStateTime = 0;
        base.OnExit();
        target.wasAttacked = false;
    }

    public override void OnUpdate()
    {
        if (target.M_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;
        if (!target.IsAlive)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
        }
        if (target.FindPlayer(100f))
        {
            target.M_StateHandler.TransitionTo(new FollowState(target));
        }

    }

}
