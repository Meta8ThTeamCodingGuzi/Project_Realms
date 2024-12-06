using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : State<Monster>
{
    public FollowState(Monster target) : base(target)
    {
        this.target = target;
    }

    public override void OnEnter()
    {
        Debug.Log($"ŸŶ  : {target.Target} , ���� �ȷο콺����Ʈ ���� ");
        target.StopMoving();
        target.Animator.SetBool("Move", true);
    }

    public override void OnExit()
    {
        target.Animator.SetBool("Move", false);
    }

    public override void OnUpdate()
    {
        if (target is Dragon dragon)
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new DragonTakeDamageState(dragon));
                return;
            }
            if (target.CanAttack(target.Target))
            {
                target.M_StateHandler.TransitionTo(new DragonAttackState(dragon, dragon.CurrentSkill.data.skillID));
                return;
            }
            if (!target.FindPlayer(15f))
            {
                target.Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new DragonIdleState(dragon));
                return;
            }
        }
        else
        {
            if (target.wasAttacked)
            {
                target.M_StateHandler.TransitionTo(new MonsterTakeDamageState(target));
                return;
            }
            Debug.Log($"{this} CanAttack �غ�");
            if (target.CanAttack(target.Target))
            {
                Debug.Log($"{this} CanAttack ȣ��");
                target.M_StateHandler.TransitionTo(new MonsterAttackState(target));
                return;
            }
            if (!target.FindPlayer(10f))
            {
                target.Animator.SetTrigger("Idle");
                target.M_StateHandler.TransitionTo(new MonsterIdleState(target));return;
            }
        }
        if (target.Target != null&&!target.CanAttack(target.Target)) target.targetMove(target.Target);
        return;
    }
}
