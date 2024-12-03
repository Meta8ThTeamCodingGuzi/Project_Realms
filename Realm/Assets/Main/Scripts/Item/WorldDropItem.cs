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

    private SphereCollider coll;
    private Vector3 startPosition;
    private bool isPlayerInRange;
    private Player player;
    private Camera mainCamera;

    private void Start()
    {
        if (item == null) item = GetComponent<Item>();

        startPosition = transform.position;
        mainCamera = Camera.main;
        coll = GetComponent<SphereCollider>();

        coll.radius = nameDisplayRadius;

        // �ʱ⿡�� �̸� �ؽ�Ʈ ��Ȱ��ȭ
        if (tooltipCanvas != null)
        {
            tooltipCanvas.gameObject.SetActive(false);
            UpdateItemNameText();
        }
    }

    private void Update()
    {
        // ������ ���Ʒ��� �յ� ���ٴϴ� ȿ��
        float newY = startPosition.y + floatingHeight + Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (isPlayerInRange)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= nameDisplayRadius)
            {
                CheckMouseHover();

                // Ŭ������ ������ ȹ��
                if (distanceToPlayer <= interactionRadius && Input.GetMouseButtonDown(0))
                {
                    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100f, mouseRaycastLayer) && hit.collider.gameObject == gameObject)
                    {
                        TryPickupItem();
                    }
                }
            }
            else
            {
                HideTooltip();
            }
        }
    }

    private void CheckMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, mouseRaycastLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                ShowTooltip();
            }
            else
            {
                HideTooltip();
            }
        }
        else
        {
            HideTooltip();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.GetComponent<Player>();
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            HideTooltip();
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
}