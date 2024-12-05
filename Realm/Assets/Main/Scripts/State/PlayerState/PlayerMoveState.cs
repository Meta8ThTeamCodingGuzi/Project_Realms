using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (target.skillController.CurrentSkill != null &&
            target.skillController.CurrentSkill.IsSkillInProgress)
        {
            return;
        }

        if (target.wasAttacked)
        {
            target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
            return;
        }

        target.InputManager.HandleInput();

        if (target.TargetMonster != null)
        {
            float distanceToTarget = Vector3.Distance(target.transform.position, target.TargetMonster.transform.position);
            float attackRange = target.CharacterStats.GetStatValue(StatType.AttackRange);

            if (distanceToTarget <= attackRange)
            {
                target.StopMoving();
                target.transform.LookAt(target.TargetMonster.transform);
                target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
                return;
            }
            target.MoveTo(target.TargetMonster.transform.position);
        }
        else if (target.TargetPos != Vector3.zero)
        {
            target.MoveTo(target.TargetPos);
        }

        if (target.HasReachedDestination())
        {
            target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
        }
    }
}
