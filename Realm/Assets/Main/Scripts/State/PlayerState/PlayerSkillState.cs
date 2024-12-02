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
        target.PlayerAnimator.SetFloat("AttackSpeed",target.CharacterStats.GetStatValue(StatType.AttackSpeed));


        target.PlayerAnimator.SetTrigger("Attack");
    }
    public override void OnExit()
    {
        base.OnExit();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }

}
