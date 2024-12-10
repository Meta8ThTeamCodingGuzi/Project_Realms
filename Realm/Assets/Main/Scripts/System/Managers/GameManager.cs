using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : SingletonManager<GameManager>, IInitializable
{
    private Player Player;
    public Player player { get => Player; set => Player = value; }
    public bool IsInitialized { get; private set; }
    public List<Monster> monster;

    [SerializeField]
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    private int currentCheckpointId = -1;

    public event Action<int> OnCheckpointReached;

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
        Checkpoint[] foundCheckpoints = FindObjectsOfType<Checkpoint>();
        checkpoints.AddRange(foundCheckpoints);

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

    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnResume()
    {
        UIManager.instance.pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    #region Checkpoint System

    public void TriggerCheckpoint(int checkpointId)
    {
        currentCheckpointId = checkpointId;
        OnCheckpointReached?.Invoke(checkpointId);
        SaveCheckpoint();
    }

    public void ResetAllCheckpoints()
    {
        currentCheckpointId = -1;
        foreach (var checkpoint in checkpoints)
        {
            checkpoint.ResetCheckpoint();
        }
    }

    public int GetCurrentCheckpointId()
    {
        return currentCheckpointId;
    }

    private void SaveCheckpoint()
    {
        PlayerPrefs.SetInt("LastCheckpoint", currentCheckpointId);
        PlayerPrefs.Save();
    }

    public void LoadCheckpoint()
    {
        currentCheckpointId = PlayerPrefs.GetInt("LastCheckpoint", -1);
    }

    #endregion
}
