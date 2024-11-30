using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : State<Player>
{
    public PlayerMoveState(Player target) : base(target)
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
