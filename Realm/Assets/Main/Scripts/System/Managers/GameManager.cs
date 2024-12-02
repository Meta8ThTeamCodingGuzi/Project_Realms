using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonManager<GameManager>, IInitializable
{
    private Player Player;
    public Player player { get => Player; set => Player = value; }
    public bool IsInitialized { get; private set; }

    protected override void Awake()
    {
        IsInitialized = false;
        base.Awake();
        StartCoroutine(InitializeRoutine());
    }

    public void Initialize()
    {
        if (!IsInitialized)
        {
            StartCoroutine(InitializeRoutine());
        }
    }

    private IEnumerator InitializeRoutine()
    {
        yield return new WaitUntil(() => CheckRequiredComponents());

        IsInitialized = true;
        Debug.Log("GameManager initialized successfully");
    }

    private bool CheckRequiredComponents()
    {
        if (player == null) return false;
        if (!player.IsInitialized) return false;

        return true;
    }
}
