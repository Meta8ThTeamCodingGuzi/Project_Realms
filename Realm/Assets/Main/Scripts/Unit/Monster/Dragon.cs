using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Monster
{
    protected override void Initialize()
    {
        base.Initialize();

        AnimController = gameObject.GetComponent<AnimatorController>();
    }



}
