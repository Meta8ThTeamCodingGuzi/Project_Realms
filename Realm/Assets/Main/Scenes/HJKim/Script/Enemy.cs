using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float enemyHp;
    public bool isDead;

    public void Start()
    {
        isDead = false;

    }
    public void TakeDamage(float damage)
    {
        print("�ƾ�");
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
