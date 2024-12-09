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
    private InventoryUI inventory;
    private PlayerStateUI playerStateUI;
    private SkillTreeUI skillTreeUI;
    private SkillSelectUI skillSelectUI;

    public PlayerUI PlayerUI => playerUI;

    public SkillSelectUI SkillSelectUI => skillSelectUI;

    private bool isInventoryVisible = false;
    private bool isSkillTreeVisible = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
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
            GameManager.instance.IsInitialized &&
            SkillManager.instance != null);

        player = GameManager.instance.player;

        GetReferences();

        playerUI.Initialize(player);
        skillTreeUI = playerUI.skillTreeUI;
        inventory = playerUI.inventoryUI;
        skillBarUI = playerUI.playerBarUI.skillBarUI;
        playerStateUI.Initialize(player);
        SetInitialUIState();

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

    private void SetInitialUIState()
    {
        inventory.gameObject.SetActive(false);
        playerUI.HideStatUI();
        isInventoryVisible = false;
    }

    private void GetReferences()
    {
        playerUI = GetComponentInChildren<PlayerUI>();
        inventory = GetComponentInChildren<InventoryUI>();
        playerStateUI = GetComponentInChildren<PlayerStateUI>();
    }

    private void Update()
    {
        if (!IsInitialized) return;

        playerUI.UpdatePlayerInfo();

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventoryAndStatUI();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            ToggleSkillTreeUI();
        }
    }

    private void ToggleInventoryAndStatUI()
    {
        isInventoryVisible = !isInventoryVisible;
        inventory.gameObject.SetActive(isInventoryVisible);

        if (isInventoryVisible)
            playerUI.ShowStatUI();
        else
            playerUI.HideStatUI();
    }
    private void ToggleSkillTreeUI()
    {
        isSkillTreeVisible = !isSkillTreeVisible;
        skillTreeUI.gameObject.SetActive(isSkillTreeVisible);
        if (isSkillTreeVisible)
        {
            playerUI.ShowSkillTreeUI();
        }
        else
        {
            playerUI.HideSkillTreeUI();
        }

    }

    public void RegisterSkillSelectUI(SkillSelectUI selectUI)
    {
        skillSelectUI = selectUI;
    }
}
