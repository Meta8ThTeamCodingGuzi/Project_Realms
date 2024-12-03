using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTakeDamageState : State<Monster>
{
    public MonsterTakeDamageState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.M_Animator.SetTrigger("TakeDamage");
    }

    public override void OnExit()
    {
        target.wasAttacked = false;
    }

    public override void OnUpdate()
    {
        if (!target.IsAlive)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
        }
        else if (target.IsAlive)
        {
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
    }

}
