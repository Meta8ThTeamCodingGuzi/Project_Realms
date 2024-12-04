using UnityEngine;
using TMPro;

public class WorldDropItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Item item;
    [SerializeField] private Canvas tooltipCanvas;
    [SerializeField] private TextMeshProUGUI itemNameText;

    [Header("Settings")]
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private float nameDisplayRadius = 5f;
    [SerializeField] private float floatingHeight = 0.5f;
    [SerializeField] private float bobSpeed = 1f;
    [SerializeField] private float bobAmount = 0.1f;
    [SerializeField] private LayerMask mouseRaycastLayer;

    private Vector3 startPosition;
    private bool isPlayerInRange;
    private Player player;
    private Camera mainCamera;
    private bool isMouseOver = false;
    private static WorldDropItem currentHoveredItem = null;

    [Header("Spawn Animation")]
    [SerializeField] private float spawnJumpHeight = 2f;
    [SerializeField] private float spawnRotationSpeed = 720f;
    [SerializeField] private float spawnAnimationTime = 0.5f;
    private float spawnStartTime;
    private bool isSpawning = true;
    private Vector3 originalRotation;

    private void Start()
    {
        if (item == null) item = GetComponent<Item>();

        startPosition = transform.position;
        mainCamera = Camera.main;
        player = GameManager.Instance.player;

        // ���� �ִϸ��̼� �ʱ�ȭ
        spawnStartTime = Time.time;
        originalRotation = transform.eulerAngles;
        isSpawning = true;

        // �ʱ⿡�� �̸� �ؽ�Ʈ ��Ȱ��ȭ
        if (tooltipCanvas != null)
        {
            tooltipCanvas.gameObject.SetActive(false);
            UpdateItemNameText();
        }
    }

    private void Update()
    {
        if (isSpawning)
        {
            UpdateSpawnAnimation();
        }
        else
        {
            // ������ �յ� ���ٴϴ� ȿ��
            float newY = startPosition.y + floatingHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // ���콺 ����ĳ��Ʈ üũ�� ������ �̸� ǥ�� (�Ÿ� ���� ����)
        CheckMouseHover();

        // Ŭ������ ������ ȹ�� (�Ÿ� ���� ����)
        if (distanceToPlayer <= interactionRadius && Input.GetMouseButtonDown(0) && isMouseOver)
        {
            TryPickupItem();
        }
    }

    private void CheckMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, mouseRaycastLayer);

        // ���� ����� ������ ã��
        float closestDistance = float.MaxValue;
        bool foundThis = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject == gameObject)
            {
                float distance = Vector3.Distance(hit.point, mainCamera.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    foundThis = true;
                }
            }
        }

        // ���콺 ���� ���� ������Ʈ
        if (foundThis && currentHoveredItem != this)
        {
            if (currentHoveredItem != null)
            {
                currentHoveredItem.HideTooltip();
            }
            currentHoveredItem = this;
            ShowTooltip();
            isMouseOver = true;
        }
        else if (!foundThis && isMouseOver)
        {
            HideTooltip();
            if (currentHoveredItem == this)
            {
                currentHoveredItem = null;
            }
            isMouseOver = false;
        }
    }

    private void ShowTooltip()
    {
        if (tooltipCanvas != null)
        {
            tooltipCanvas.gameObject.SetActive(true);
            tooltipCanvas.transform.forward = mainCamera.transform.forward;
        }
    }

    private void HideTooltip()
    {
        if (tooltipCanvas != null)
        {
            tooltipCanvas.gameObject.SetActive(false);
        }
    }

    private void UpdateItemNameText()
    {
        if (itemNameText != null && item != null)
        {
            ItemInstanceData instanceData = ItemManager.Instance.GetItemInstanceData(item);
            if (instanceData != null)
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(instanceData.NameColor);
                itemNameText.text = $"<color=#{colorHex}>[{instanceData.Rarity}] {item.ItemID}</color>";
            }
        }
    }

    private void TryPickupItem()
    {
        if (player == null) return;

        PlayerInventorySystem inventorySystem = player.InventorySystem;
        if (inventorySystem == null)
        {
            Debug.LogError("PlayerInventorySystem not found!");
            return;
        }

        // ���ο� ������ �ν��Ͻ� ����
        GameObject newItemObj = new GameObject(item.ItemID.ToString());
        Item newItem = newItemObj.AddComponent<Item>();

        // ������ �����Ϳ� �ν��Ͻ� ������ ����
        ItemInstanceData instanceData = ItemManager.Instance.GetItemInstanceData(item);
        if (instanceData == null)
        {
            Debug.LogError("Item instance data not found!");
            Destroy(newItemObj);
            return;
        }

        // �� ������ �ʱ�ȭ
        newItem.Initialize(item.ItemData, instanceData);
        newItemObj.transform.SetParent(player.transform);

        // PlayerInventorySystem�� ���� ������ �߰�
        if (inventorySystem.AddItem(newItem))
        {
            // ���������� �߰��Ǿ��ٸ� ���忡�� ����
            ItemManager.Instance.RemoveItem(item);
            Destroy(gameObject);
        }
        else
        {
            // ���н� ������ ������ ����
            Destroy(newItemObj);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // �����Ϳ��� ��ȣ�ۿ� ������ �̸� ǥ�� ������ �ð�ȭ
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, nameDisplayRadius);
    }

    private void UpdateSpawnAnimation()
    {
        float elapsedTime = Time.time - spawnStartTime;
        float progress = elapsedTime / spawnAnimationTime;

        if (progress >= 1f)
        {
            isSpawning = false;
            transform.eulerAngles = originalRotation;
            return;
        }

        float jumpProgress = Mathf.Sin(progress * Mathf.PI);
        float currentHeight = startPosition.y + (spawnJumpHeight * jumpProgress);

        float rotationAmount = spawnRotationSpeed * elapsedTime;

        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        transform.eulerAngles = new Vector3(
            originalRotation.x,
            originalRotation.y,
            originalRotation.z + rotationAmount
        );
    }
}