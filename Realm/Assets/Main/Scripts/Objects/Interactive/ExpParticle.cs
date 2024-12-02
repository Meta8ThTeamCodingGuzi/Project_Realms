using UnityEngine;

public class ExpParticle : MonoBehaviour
{
    [SerializeField] private float expAmount = 10f;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float accelerationRate = 2f; // 플레이어에게 가까워질수록 가속

    private Player player;
    private bool isAttracting = false;
    private float currentSpeed;

    private void Start()
    {
        player = GameManager.Instance.player;
        currentSpeed = moveSpeed;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float expRange = player.CharacterStats.GetStatValue(StatType.ExpRange);

        // ExpRange 범위 안에 들어왔는지 체크
        if (distanceToPlayer <= expRange)
        {
            isAttracting = true;
        }

        if (isAttracting)
        {
            currentSpeed = moveSpeed + (moveSpeed * accelerationRate * (1 - distanceToPlayer / expRange));

            Vector3 direction = (player.transform.position - transform.position).normalized;
            transform.position += direction * currentSpeed * Time.deltaTime;

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