using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : State<Player>
{
    public PlayerSkillState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.StopMoving();
        target.PlayerAnimator.SetFloat("AttackSpeed",target.CharacterStats.GetStatValue(StatType.AttackSpeed) * 12f);


        target.PlayerAnimator.SetTrigger("Attack");
    }
    public override void OnExit()
    {
        
    }
    public override void OnUpdate()
    {
        while (target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            return;
        }

        while (target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return;
        }

        target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
    }

}
