using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamageState : State<Player>
{
    public PlayerTakeDamageState(Player target) : base(target)
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
