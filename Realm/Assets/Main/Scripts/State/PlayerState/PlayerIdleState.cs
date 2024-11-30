using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : State<Player>
{
    public PlayerIdleState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {

    }
}
