using UnityEngine;

public class ExpParticle : MonoBehaviour
{
    [SerializeField] private float expAmount = 10f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationRate = 2f; // �÷��̾�� ����������� ����

    // �յ� ���ٴϴ� ȿ���� ���� ���ο� ������
    [SerializeField] private float floatAmplitude = 0.3f; // ���Ʒ��� �����̴� ����
    [SerializeField] private float floatFrequency = 2f;   // �������� �ֱ�
    private Vector3 startPosition;
    private float floatTimer;

    private Player player;
    private bool isAttracting = false;
    private float currentSpeed;

    private void Start()
    {
        player = GameManager.Instance.player;
        currentSpeed = moveSpeed;
        startPosition = transform.position;
        floatTimer = Random.Range(0f, 2f * Mathf.PI); // ������ ���� ��ġ
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float expRange = player.CharacterStats.GetStatValue(StatType.ExpRange);

        // �յ� ���ٴϴ� ȿ�� ����
        if (!isAttracting)
        {
            floatTimer += Time.deltaTime;
            float yOffset = Mathf.Sin(floatTimer * floatFrequency) * floatAmplitude;
            Vector3 newPosition = startPosition + new Vector3(0f, yOffset, 0f);

            // ��ġ ���� ��ȿ���� Ȯ��
            if (!float.IsInfinity(newPosition.x) && !float.IsInfinity(newPosition.y) && !float.IsInfinity(newPosition.z))
            {
                transform.position = newPosition;
            }
        }

        // ExpRange ���� �ȿ� ���Դ��� üũ
        if (distanceToPlayer <= expRange)
        {
            isAttracting = true;
        }

        if (isAttracting)
        {
            // �ӵ� ��� �� 0���� ������ ���� ����
            float speedMultiplier = expRange > 0.001f ? (1 - distanceToPlayer / expRange) : 1f;
            currentSpeed = moveSpeed + (moveSpeed * accelerationRate * Mathf.Clamp01(speedMultiplier));

            Vector3 direction = (player.transform.position - transform.position).normalized;
            Vector3 movement = direction * currentSpeed * Time.deltaTime;

            // �̵� ���Ͱ� ��ȿ���� Ȯ��
            if (!float.IsInfinity(movement.x) && !float.IsInfinity(movement.y) && !float.IsInfinity(movement.z))
            {
                transform.position += movement;
            }

            if (distanceToPlayer < 0.5f)
            {
                player.GainExperience(expAmount);
                PoolManager.Instance.Despawn<ExpParticle>(this);
            }
        }
    }

    // ����ġ �� ���� �޼���
    public void SetExpAmount(float amount)
    {
        expAmount = amount;
    }

#if UNITY_EDITOR
    // �ð��� ������� ���� �����
    private void OnDrawGizmos()
    {
        if (player != null && Application.isPlaying)
        {
            float expRange = player.CharacterStats.GetStatValue(StatType.ExpRange);
            Gizmos.color = isAttracting ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
#endif
}