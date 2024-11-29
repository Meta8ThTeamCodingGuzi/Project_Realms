using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : State<Monster>
{

    public MonsterAttackState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {                
        target.Attack(target.targetPlayer);
        target.StopMoving();
        target.M_Animator.SetTrigger("Attack");
        base.OnEnter();
    }

    public override void OnExit()
    {
        target.StopAttack();
        target.wasAttacked = true;
        base.OnExit();
    }

    public override void OnUpdate()
    {
        if (target.M_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;
        if (!target.IsAlive)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
        }
        if (target.wasAttacked)
        {
            target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
        }
        if (!target.wasAttacked || !target.CanAttack(target.targetPlayer))
        {
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
    }

}
