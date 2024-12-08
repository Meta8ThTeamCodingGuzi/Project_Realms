using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterSpawnData
{
    public Transform spawnPoint;
    public Monster monsterPrefab;
    public int requiredCheckpointId; 
    public float spawnDelay = 3f;    
    public bool hasSpawned = false;
}

public class MonsterManager : SingletonManager<MonsterManager>
{
    [SerializeField]
    private List<MonsterSpawnData> spawnDataList = new List<MonsterSpawnData>();

    private int currentCheckpoint = 0;

    private void Start()
    {
        GameManager.Instance.OnCheckpointReached += HandleCheckpoint;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnCheckpointReached -= HandleCheckpoint;
    }

    private void HandleCheckpoint(int checkpointId)
    {
        currentCheckpoint = checkpointId;
        CheckAndSpawnMonsters();
    }

    private void CheckAndSpawnMonsters()
    {
        foreach (var spawnData in spawnDataList)
        {
            if (!spawnData.hasSpawned && spawnData.requiredCheckpointId <= currentCheckpoint)
            {
                StartCoroutine(SpawnMonsterWithDelay(spawnData));
            }
        }
    }

    private IEnumerator SpawnMonsterWithDelay(MonsterSpawnData spawnData)
    {
        yield return new WaitForSeconds(spawnData.spawnDelay);

        Monster monster = PoolManager.Instance.Spawn<Monster>(
            spawnData.monsterPrefab.gameObject,
            spawnData.spawnPoint.position,
            Quaternion.identity
        );

        monster.Initialize();
        spawnData.hasSpawned = true;
    }

    public void AddSpawnData(MonsterSpawnData spawnData)
    {
        spawnDataList.Add(spawnData);
    }

    public void ResetSpawnData()
    {
        foreach (var spawnData in spawnDataList)
        {
            spawnData.hasSpawned = false;
        }
        currentCheckpoint = 0;
    }
}