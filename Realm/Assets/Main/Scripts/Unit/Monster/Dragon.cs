using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Monster
{
    public UnitState dragonState { get; set; } = UnitState.Ground;

    private bool FlyState = false;

    public float dragonHp => this.characterStats.GetStatValue(StatType.Health);

    public override void Initialize()
    {
        base.Initialize();

        AnimController = gameObject.GetComponent<AnimatorController>();
        AnimController.Initialize(this);

        GetSkill(SkillID.DragonBite);
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
}
