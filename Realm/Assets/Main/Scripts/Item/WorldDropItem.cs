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

        // 스폰 애니메이션 초기화
        spawnStartTime = Time.time;
        originalRotation = transform.eulerAngles;
        isSpawning = true;

        // 초기에는 이름 텍스트 비활성화
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
            // 기존의 둥둥 떠다니는 효과
            float newY = startPosition.y + floatingHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // 마우스 레이캐스트 체크로 아이템 이름 표시 (거리 제한 없음)
        CheckMouseHover();

        // 클릭으로 아이템 획득 (거리 제한 있음)
        if (distanceToPlayer <= interactionRadius && Input.GetMouseButtonDown(0) && isMouseOver)
        {
            TryPickupItem();
        }
    }

    private void CheckMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, mouseRaycastLayer);

        // 가장 가까운 아이템 찾기
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

        // 마우스 오버 상태 업데이트
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

        // 새로운 아이템 인스턴스 생성
        GameObject newItemObj = new GameObject(item.ItemID.ToString());
        Item newItem = newItemObj.AddComponent<Item>();

        // 아이템 데이터와 인스턴스 데이터 복사
        ItemInstanceData instanceData = ItemManager.Instance.GetItemInstanceData(item);
        if (instanceData == null)
        {
            Debug.LogError("Item instance data not found!");
            Destroy(newItemObj);
            return;
        }

        // 새 아이템 초기화
        newItem.Initialize(item.ItemData, instanceData);
        newItemObj.transform.SetParent(player.transform);

        // PlayerInventorySystem을 통해 아이템 추가
        if (inventorySystem.AddItem(newItem))
        {
            // 성공적으로 추가되었다면 월드에서 제거
            ItemManager.Instance.RemoveItem(item);
            Destroy(gameObject);
        }
        else
        {
            // 실패시 생성한 아이템 제거
            Destroy(newItemObj);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 에디터에서 상호작용 범위와 이름 표시 범위를 시각화
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