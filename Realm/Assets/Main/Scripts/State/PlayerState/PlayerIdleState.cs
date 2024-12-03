using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.EventSystems;

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

        if (Input.GetMouseButton(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.TryGetComponent<Monster>(out Monster monster))
                {
                    target.SetTarget(monster);

                    float distanceToTarget = Vector3.Distance(target.transform.position, monster.transform.position);
                    float attackRange = target.CharacterStats.GetStatValue(StatType.AttackRange);

                    if (distanceToTarget <= attackRange)
                    {
                        target.skillController.OnMouseClick();
                        target.PlayerHandler.TransitionTo(new PlayerSkillState(target));
                    }
                    else
                    {
                        target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
                    }
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, target.GroundLayer))
            {
                target.SetDestination(hit.point);
                target.PlayerHandler.TransitionTo(new PlayerMoveState(target));
            }
        }
    }
}
