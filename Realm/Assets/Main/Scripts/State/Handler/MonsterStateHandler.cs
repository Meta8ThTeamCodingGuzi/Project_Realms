using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStateHandler : StateHandler<Monster>
{
    public MonsterStateHandler(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void Initialize()
    {
        CurrentState = new MonsterIdleState(target);
        CurrentState.OnEnter();   
        base.Initialize();
    }
}
