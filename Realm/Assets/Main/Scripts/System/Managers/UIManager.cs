using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : SingletonManager<UIManager>, IInitializable
{
    public bool IsInitialized { get; private set; }

    private PlayerUI playerUI;
    private SkillBarUI skillBarUI;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    public void Initialize()
    {
        StartCoroutine(InitializeRoutine());
    }

    public IEnumerator InitializeRoutine()
    {
        yield return new WaitUntil(() =>
            GameManager.instance != null &&
            GameManager.instance.IsInitialized);

        player = GameManager.instance.player;

        //UI �ϼ��� ��������
        GetReferences();
        playerUI.Initialize(player);
        skillBarUI.Initialize(player);
        //TODO : UI �ϼ� �� �������� , ���� �޾ƿ��� �κ��� �Ѳ����� �ѱ��
        //Instantiate(playerUI.gameObject, transform);

        if (player != null)
        {
            IsInitialized = true;
            Debug.Log("UIManager initialized successfully");
        }
        else
        {
            Debug.LogError("UIManager initialization failed: Player is null");
        }
    }

    private void GetReferences()
    {
        playerUI = GetComponentInChildren<PlayerUI>();
        skillBarUI = GetComponentInChildren<SkillBarUI>();
    }

    private void Update()
    {
        if (IsInitialized)
        {
            playerUI.UpdatePlayerInfo();
        }
    }
}
