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

        // �Ϲ� ��ų �Է� üũ
        if (target.skillController.CheckSkillInputs())
        {
            target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
            return;
        }

        // Ÿ�� ���Ͱ� �ִ� ���
        if (target.TargetMonster != null)
        {
            // ���� ���� ���� ���Դ��� üũ
            if (target.CanAttack(target.TargetMonster))
            {
                target.skillController.OnMouseClick();
                target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
                return;
            }
            // ���� ���� ���̸� ���͸� ���� �̵�
            target.MoveTo(target.TargetMonster.transform.position);
        }
        // �̵� ��ǥ ������ �ִ� ���
        else if (target.TargetPos != Vector3.zero)
        {
            target.MoveTo(target.TargetPos);
        }

        // �������� �����ߴ��� üũ
        if (target.HasReachedDestination())
        {
            target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
        }
    }
}
