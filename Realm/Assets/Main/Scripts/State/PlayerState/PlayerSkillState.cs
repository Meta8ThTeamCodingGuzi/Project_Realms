using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillState : State<Player>
{
    private bool isAttackAnimationPlaying = false;

    public PlayerSkillState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.StopMoving();
        isAttackAnimationPlaying = true;
    }

    public override void OnExit()
    {
        target.ClearTarget();
        isAttackAnimationPlaying = false;
    }

    public override void OnUpdate()
    {
        if (target.wasAttacked)
        {
            target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
            return;
        }

        AnimatorStateInfo stateInfo = target.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack"))
        {
            if (stateInfo.normalizedTime < 0.97f)
            {
                return;
            }
            else
            {
                isAttackAnimationPlaying = false;
            }
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
