using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : State<Player>
{
    public PlayerAttackState(Player target) : base(target)
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
