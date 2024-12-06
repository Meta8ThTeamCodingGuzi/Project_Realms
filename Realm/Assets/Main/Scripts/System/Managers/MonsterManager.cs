using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonManager<MonsterManager>
{
    public Monster monsterPrefab;
    public Transform spawnpos;

    public void SpawnMob() 
    {
        Monster monster = PoolManager.Instance.Spawn<Monster>(monsterPrefab.gameObject, spawnpos.position, Quaternion.identity);
        monster.Initialize();
    }
}
