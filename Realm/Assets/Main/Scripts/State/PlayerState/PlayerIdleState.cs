using System.Collections;
using System.Collections.Generic;
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

    }

    public override void OnExit()
    {
        
    }

    public override void OnUpdate()
    {
        IdleTime += Time.deltaTime;
        if (IdleTime > 5f)
        {
            target.PlayerAnimator.SetTrigger("FoldWeapon");
            IdleTime = 0;
        }
        if (target.PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            target.PlayerAnimator.SetTrigger("SetWeapon");
        }


    }







}
