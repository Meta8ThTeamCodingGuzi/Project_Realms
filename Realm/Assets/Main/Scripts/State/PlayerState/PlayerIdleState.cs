using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class PlayerIdleState : State<Player>
{

    public PlayerIdleState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.StopMoving();
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        if (!target.IsAlive)
        {
            target.PlayerHandler.TransitionTo(new PlayerDieState(target));
            return;
        }

        AnimatorStateInfo currentState = target.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (target.wasAttacked)
        {
            target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
            return;
        }
        if (target.skillController.CheckSkillInputs())
        {
            target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
            return;
        }

        target.MovetoCursor();

        if(target.TargetMonster != null && target.CanAttack(target.TargetMonster))
        {
            target.skillController.OnMouseClick();
            target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
        }
        if (target.TargetPos != Vector3.zero)
        {
            target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
            return;
        }
        else if (target.TargetMonster != null)
        {
            target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
        }

    }
}
