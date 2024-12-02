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
        target.PlayerAnimator.SetFloat("AttackSpeed", target.CharacterStats.GetStatValue(StatType.AttackSpeed)/2f);


        target.PlayerAnimator.SetTrigger("Attack");
    }
    public override void OnExit()
    {

    }
    public override void OnUpdate()
    {
        var currentAnimatorState = target.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (currentAnimatorState.normalizedTime >= 0.3f)
        {
            if (target.TargetPos != Vector3.zero || target.TargetMonster != null)
            {
                target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
            }
            else
            {
                target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
            }
        }
    }

}
