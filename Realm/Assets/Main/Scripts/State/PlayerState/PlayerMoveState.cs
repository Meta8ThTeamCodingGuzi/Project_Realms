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
        target.PlayerAnimator.SetFloat("MoveSpeed",target.CharacterStats.GetStatValue(StatType.MoveSpeed)/4.5f);
        target.PlayerAnimator.SetBool("Move",true);
    }

    public override void OnExit()
    {
        target.PlayerAnimator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {
        if (target.wasAttacked)
        {
            target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
        }
        if (target.skillController.CheckSkillInputs())
        {
            target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
        }
        target.MovetoCursor();

        if(target.TargetPos != Vector3.zero) target.MoveTo(target.TargetPos);

        if (target.HasReachedDestination())
        {
            target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
        }
    }
}
