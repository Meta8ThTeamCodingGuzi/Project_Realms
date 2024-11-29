using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDieState : State<Player>
{
    public PlayerDieState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {

    }
}
