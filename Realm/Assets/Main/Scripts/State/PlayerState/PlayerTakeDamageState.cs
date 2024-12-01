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
        target.wasAttacked = false;
        target.PlayerAnimator.SetTrigger("TakeDamage");
    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        if (target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("TakeDamage"))
        {
            if (target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                return;
            }
        }        
        if (!target.IsAlive)
        { 
            target.PlayerHandler.TransitionTo(new PlayerDieState(target));
        }
        else
        {
            target.PlayerAnimator.SetTrigger("Idle");
            target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
        }
    }
}
