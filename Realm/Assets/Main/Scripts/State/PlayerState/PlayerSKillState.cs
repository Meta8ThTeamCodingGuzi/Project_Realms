using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSKillState : State<Player>
{
    public PlayerSKillState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        //target.PlayerAnimator.SetFloat("AttackSpeed", target.CharacterStats.GetStatValue(StatType.AttackSpeed) / 2f);
        target.PlayerAnimator.SetTrigger("Attack");
    }

    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {


        if (target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            if(target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            {
                return;
            }
            else if (target.wasAttacked)
            {
                target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
            }
            else
            {
                target.PlayerAnimator.SetTrigger("Ilde");
                target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
            }
        }
    }
}
