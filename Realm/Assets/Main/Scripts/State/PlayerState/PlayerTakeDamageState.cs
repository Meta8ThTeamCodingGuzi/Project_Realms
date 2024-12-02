using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeDamageState : State<Player>
{
    public PlayerTakeDamageState(Player target) : base(target)
    {
        this.target = target;
    }
}
