using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdleState : State<Player>
{
    public PlayerIdleState(Player target) : base(target)
    {
        this.target = target;
    }

    private float IdleTime = 0;

    public override void OnEnter()
    {
        target.StopMoving();
    }

    public override void OnExit()
    {
        IdleTime = 0;
    }

    public override void OnUpdate()
    {
        //if (target.skillController.IsExistSkill())
        //{
        //    target.PlayerHandler.TransitionTo(new PlayerSKillState(target));
        //}
        if (target.wasAttacked)
        {
            target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
        }
        IdleTime += Time.deltaTime;
        if (IdleTime > 15f)
        {
            target.PlayerAnimator.SetTrigger("FoldWeapon");
            IdleTime = 0;
        }
        if (!target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("SetWeapon"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                target.PlayerAnimator.SetTrigger("SetWeapon");
                return;
            }
            return;
        }
        else
        {
            target.MovetoCursor();
            if (target.TargetPos != Vector3.zero)
            {
                target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
            }



        }

    }

}
