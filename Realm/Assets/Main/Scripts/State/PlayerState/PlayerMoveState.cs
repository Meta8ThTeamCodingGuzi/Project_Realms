using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PlayerMoveState : State<Player>
{
    public PlayerMoveState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.PlayerAnimator.SetFloat("MoveSpeed", target.CharacterStats.GetStatValue(StatType.MoveSpeed) / 4.5f);
        target.PlayerAnimator.SetBool("Move", true);
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
            return;
        }

        // 일반 스킬 입력 체크
        if (target.skillController.CheckSkillInputs())
        {
            target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
            return;
        }

        // 타겟 몬스터가 있는 경우
        if (target.TargetMonster != null)
        {
            // 공격 범위 내에 들어왔는지 체크
            if (target.CanAttack(target.TargetMonster))
            {
                target.skillController.OnMouseClick();
                target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
                return;
            }
            // 아직 범위 밖이면 몬스터를 향해 이동
            target.MoveTo(target.TargetMonster.transform.position);
        }
        // 이동 목표 지점이 있는 경우
        else if (target.TargetPos != Vector3.zero)
        {
            target.MoveTo(target.TargetPos);
        }

        // 목적지에 도착했는지 체크
        if (target.HasReachedDestination())
        {
            target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
        }
    }
}
