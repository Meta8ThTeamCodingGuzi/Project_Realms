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
        if (player == null || player.CharacterStats == null) return;

        // �յ� ���ٴϴ� ȿ�� ����
        if (!isAttracting)
        {
            floatTimer += Time.deltaTime;
            float yOffset = Mathf.Sin(floatTimer * floatFrequency) * floatAmplitude;
            transform.position = startPosition + new Vector3(0f, yOffset, 0f);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float expRange = player.CharacterStats.GetStatValue(StatType.ExpRange);

        // expRange�� 0������ ��� ó��
        if (expRange <= 0f) expRange = 0.1f;

        // ExpRange ���� �ȿ� ���Դ��� üũ
        if (distanceToPlayer <= expRange)
        {
            isAttracting = true;
        }

        if (isAttracting)
        {
            // �ӵ� ��� �� ������ġ �߰�
            float speedMultiplier = Mathf.Clamp01(1 - distanceToPlayer / expRange);
            currentSpeed = moveSpeed + (moveSpeed * accelerationRate * speedMultiplier);

            Vector3 directionVector = player.transform.position - transform.position;
            // ���� ���Ͱ� 0�� ��� üũ
            if (directionVector.sqrMagnitude > 0.001f)
            {
                Vector3 direction = directionVector.normalized;
                Vector3 newPosition = transform.position + direction * currentSpeed * Time.deltaTime;

                // ��ġ �� ��ȿ�� �˻�
                if (!float.IsInfinity(newPosition.x) && !float.IsInfinity(newPosition.y) && !float.IsInfinity(newPosition.z))
                {
                    transform.position = newPosition;
                }
                else
                {
                    Debug.LogWarning($"Invalid position detected. Current: {transform.position}, Target: {player.transform.position}");
                    isAttracting = false;
                }
            }

            if (distanceToPlayer < 0.5f)
            {
                player.GainExperience(expAmount);
                PoolManager.Instance.Despawn<ExpParticle>(this);
            }
        }

        if (distanceToPlayer > 15f)
        {
            player.GainExperience(expAmount);
            PoolManager.Instance.Despawn<ExpParticle>(this);
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