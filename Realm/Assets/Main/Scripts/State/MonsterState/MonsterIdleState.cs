using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterIdleState : State<Monster>
{
    float patrol;

    public MonsterIdleState(Monster target) : base(target) 
    { 
        this.target = target;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        patrol = 0;
    }

    public override void OnUpdate()
    {
        float patrol = Time.deltaTime;
        if(target.characterStats.)
        if (patrol > 3f)
        {
            target.M_StateGandler.TransitionTo(new MonsterMoveState(target));
        }
        if (target.FindPlayer(10f))
        {
            target.M_StateGandler.TransitionTo(new FollowState(target));
        }

    }

}
