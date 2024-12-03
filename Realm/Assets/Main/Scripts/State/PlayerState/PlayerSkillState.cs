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
    }
    public override void OnExit()
    {
        target.ClearTarget();
    }
    public override void OnUpdate()
    {
        if (target.wasAttacked)
        {
            target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
        }

        if (target.skillController.CheckSkillInputs())
        {
            return;
        }

        AnimatorStateInfo stateInfo = target.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack") && stateInfo.normalizedTime < 0.97f)
        {
            return;
        }

        if (target.TargetPos != Vector3.zero)
        {
            target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
        }
        else
        {
            target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
        }
    }

}
