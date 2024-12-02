using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : State<Player>
{
    private float IdleTime = 0f;

    public PlayerIdleState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.StopMoving();
        IdleTime = 0f;
        base.OnEnter();
    }

    public override void OnExit()
    {
        IdleTime = 0f;
        base.OnExit();
    }

    public override void OnUpdate()
    {
        AnimatorStateInfo currentState = target.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("SetWeapon"))
        {
            target.MovetoCursor();
            if (!currentState.IsName("Idle"))
            {
                IdleTime += Time.deltaTime;
                if (IdleTime > 20f) { target.PlayerAnimator.SetTrigger("FoldWeapon"); IdleTime = 0f; }
            }

            if (target.TargetPos != Vector3.zero)
            {
                target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
            }

            if(target.TargetMonster != null)
            {
                target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
            }

            if (target.skillController.CheckSkillInputs())
            {
                target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
            }
        }
        else if (currentState.IsName("Idle"))
        {
            if (Input.GetMouseButton(0)) target.PlayerAnimator.SetTrigger("SetWeapon");
        }


    }





}
