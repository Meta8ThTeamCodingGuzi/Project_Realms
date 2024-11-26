using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : State<Monster>
{
    public FollowState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        if(target.CharacterStats.GetStatValue(StatType.Health) <= 0)
        {
            target.M_StateHandler.TransitionTo(new MonsterDieState(target));
        }
        if (target.CanAttack(target.targetPlayer))
        {
            target.M_StateHandler.TransitionTo(new MonsterAttackState(target));
        }
        if (!target.FindPlayer(10f))
        {
            target.M_StateHandler.TransitionTo(new MonsterIdleState(target));
        }
        target.targetMove(target.targetPlayer);
    }
}
