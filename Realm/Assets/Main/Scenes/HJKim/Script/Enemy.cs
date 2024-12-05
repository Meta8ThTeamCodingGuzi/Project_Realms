using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyHp;
    public bool isDead;

    public void Start()
    {
        isDead = false;

    }
    public void TakeDamage(int damage)
    {
        print("¾Æ¾ß");
        enemyHp -= damage;

        if (enemyHp <= 0)
        {
            Die();

        }
    }

    public void Die()
    {
        isDead = true;
        Destroy(gameObject);

    }

}
