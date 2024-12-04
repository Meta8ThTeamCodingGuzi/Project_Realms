using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerSkillState : State<Player>
{

    public PlayerSkillState(Player target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        target.StopMoving();
    }

    public override void OnExit()
    {
        target.ClearTarget();
    }

    public override void OnUpdate()
    {
        if (target.wasAttacked)
        {
            target.PlayerHandler.TransitionTo(new PlayerTakeDamageState(target));
            return;
        }

        AnimatorStateInfo stateInfo = target.PlayerAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack"))
        {
            if (stateInfo.normalizedTime < 0.97f)
            {
                return;
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
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
                            if (target.skillController.CurrentSkill is WeaponSkill weaponSkill && !weaponSkill.IsAttacking)
                            {
                                target.skillController.OnMouseClick();
                            }
                            return;
                        }
                        else
                        {
                            target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
                            return;
                        }
                    }
                }

                if (Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, target.GroundLayer))
                {
                    target.SetDestination(groundHit.point);
                    target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
                    return;
                }
            }
        }

        if (target.TargetMonster != null)
        {
            float distanceToTarget = Vector3.Distance(target.transform.position, target.TargetMonster.transform.position);
            float attackRange = target.CharacterStats.GetStatValue(StatType.AttackRange);

            if (distanceToTarget <= attackRange)
            {
                if (target.skillController.CurrentSkill is WeaponSkill weaponSkill && !weaponSkill.IsAttacking)
                {
                    target.skillController.OnMouseClick();
                }
                return;
            }
            else
            {
                target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
                return;
            }
        }
        else if (target.TargetPos != Vector3.zero)
        {
            target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
        }
        else
        {
            target.PlayerHandler.TransitionTo(new PlayerIdleState(target));
        }
    }
}
