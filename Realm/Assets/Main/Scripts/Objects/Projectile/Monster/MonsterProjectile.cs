using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterProjectile : Projectile
{

    protected override void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Player>(out Player Player))
        {
            Player.TakeDamage(data.Damage);
            PoolManager.Instance.Despawn(this);
        }           
    }
}
