using UnityEngine;

public class ExpParticle : MonoBehaviour
{
    [SerializeField] private float expAmount = 10f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationRate = 2f; // 플레이어에게 가까워질수록 가속

    // 둥둥 떠다니는 효과를 위한 새로운 변수들
    [SerializeField] private float floatAmplitude = 0.3f; // 위아래로 움직이는 범위
    [SerializeField] private float floatFrequency = 2f;   // 움직임의 주기
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
        floatTimer = Random.Range(0f, 2f * Mathf.PI); // 랜덤한 시작 위치
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float expRange = player.CharacterStats.GetStatValue(StatType.ExpRange);

        // 둥둥 떠다니는 효과 적용
        if (!isAttracting)
        {
            floatTimer += Time.deltaTime;
            float yOffset = Mathf.Sin(floatTimer * floatFrequency) * floatAmplitude;
            Vector3 newPosition = startPosition + new Vector3(0f, yOffset, 0f);

            // 위치 값이 유효한지 확인
            if (!float.IsInfinity(newPosition.x) && !float.IsInfinity(newPosition.y) && !float.IsInfinity(newPosition.z))
            {
                transform.position = newPosition;
            }
        }

        // ExpRange 범위 안에 들어왔는지 체크
        if (distanceToPlayer <= expRange)
        {
            isAttracting = true;
        }

        if (isAttracting)
        {
            // 속도 계산 시 0으로 나누는 것을 방지
            float speedMultiplier = expRange > 0.001f ? (1 - distanceToPlayer / expRange) : 1f;
            currentSpeed = moveSpeed + (moveSpeed * accelerationRate * Mathf.Clamp01(speedMultiplier));

            Vector3 direction = (player.transform.position - transform.position).normalized;
            Vector3 movement = direction * currentSpeed * Time.deltaTime;

            // 이동 벡터가 유효한지 확인
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

    // 경험치 양 설정 메서드
    public void SetExpAmount(float amount)
    {
        expAmount = amount;
    }

#if UNITY_EDITOR
    // 시각적 디버깅을 위한 기즈모
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