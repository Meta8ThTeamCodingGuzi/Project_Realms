using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDieState : State<Monster>
{
    public MonsterDieState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        PoolManager.Instance.Despawn(target);
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        
    }
}
