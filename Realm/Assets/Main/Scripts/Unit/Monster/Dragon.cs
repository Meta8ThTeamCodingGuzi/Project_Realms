using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Monster
{
    public UnitState dragonState { get; set; } = UnitState.Ground;

    public Skill currentSkill { get; set; }

    private bool FlyState = false;

    public float dragonHp => this.characterStats.GetStatValue(StatType.Health);

    public override void Initialize()
    {
        base.Initialize();

        currentSkill = Skills[0];

        AnimController = gameObject.GetComponent<AnimatorController>();
    }


    public void DragonFormChange()
    {
        if (FlyState) return;

        if (dragonState == UnitState.Ground)
        {
            dragonState = UnitState.Fly;
        }
        else
        {
            dragonState = UnitState.Ground;
        }

        AnimController.DragonAnimatorChange(dragonState);

        FlyState = true;

    }
    public void DragonSKillChange(SkillID skillID)
    {
        currentSkill = GetSkill(skillID);
    }

}
