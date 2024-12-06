using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TooltipWindow : MonoBehaviour
{
    private static TooltipWindow instance;
    public static TooltipWindow Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TooltipWindow>();
            }
            return instance;
        }
    }

    [SerializeField] private RectTransform windowRect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private TextMeshProUGUI itemDescText;
    [SerializeField] private Image itemIconImage;
    [SerializeField] private TextMeshProUGUI itemRarityText;

    private Camera mainCamera;
    private Canvas parentCanvas;
    private Vector2 offset = new Vector2(20f, 20f);
    private RectTransform canvasRectTransform;
    private bool isShowing = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        mainCamera = Camera.main;
        parentCanvas = GetComponentInParent<Canvas>();
        canvasRectTransform = parentCanvas.GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        InitializeTooltip();
    }
    private void InitializeTooltip()
    {
        isShowing = false;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void ShowTooltip(string title, string rarity, string type, string description, string content, Sprite icon, float xPosition)
    {
        if (isShowing) return;

        isShowing = true;
        itemNameText.text = title;
        itemRarityText.text = rarity;
        itemTypeText.text = type;
        itemDescText.text = description;
        tooltipText.text = content;

        if (itemIconImage != null)
        {
            itemIconImage.sprite = icon;
            itemIconImage.gameObject.SetActive(icon != null);
        }

        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        LayoutRebuilder.ForceRebuildLayoutImmediate(windowRect);
        Canvas.ForceUpdateCanvases();

        UpdatePosition();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            UpdatePosition();
        }
    }

    private void UpdatePosition()
    {
        if (!isShowing) return;

        Vector2 mousePos = Input.mousePosition;
        Vector2 position = mousePos + offset;

        Vector2 size = windowRect.rect.size;

        if (position.x + size.x > Screen.width)
        {
            position.x = mousePos.x - size.x - offset.x;
        }

        if (position.y + size.y > Screen.height)
        {
            position.y = mousePos.y - size.y - offset.y;
        }

        position.x = Mathf.Max(position.x, offset.x);
        position.y = Mathf.Max(position.y, offset.y);

        if (parentCanvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            position = mainCamera.ScreenToWorldPoint(position);
        }

        windowRect.position = position;
    }

    public void HideTooltip()
    {
        if (!isShowing) return;

        isShowing = false;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}