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

        HandleMouseInput();

        if (target.TargetMonster != null)
        {
            float distanceToTarget = Vector3.Distance(target.transform.position, target.TargetMonster.transform.position);
            float attackRange = target.CharacterStats.GetStatValue(StatType.AttackRange);

            if (distanceToTarget <= attackRange)
            {
                target.StopMoving();
                target.transform.LookAt(target.TargetMonster.transform);
                target.skillController.OnMouseClick();
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

    private void HandleMouseInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.TryGetComponent<Monster>(out Monster monster))
                {
                    target.SetTarget(monster);
                    float distanceToTarget = Vector3.Distance(target.transform.position, monster.transform.position);
                    float attackRange = target.CharacterStats.GetStatValue(StatType.AttackRange);

                    if (distanceToTarget <= attackRange)
                    {
                        target.StopMoving();
                        target.transform.LookAt(monster.transform);
                        target.skillController.OnMouseClick();
                        target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
                        return;
                    }
                    return;
                }
            }

            if (Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, target.GroundLayer))
            {
                target.SetDestination(groundHit.point);
            }
        }
    }
}
