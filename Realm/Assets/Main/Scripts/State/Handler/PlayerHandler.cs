using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : StateHandler<Player>
{
    public PlayerHandler(Player target) : base(target)
    {
        this.target = target;
    }

    public override void Initialize()
    {
        CurrentState = new PlayerIdleState(target);
        CurrentState.OnEnter();
        base.Initialize();
    }
}
