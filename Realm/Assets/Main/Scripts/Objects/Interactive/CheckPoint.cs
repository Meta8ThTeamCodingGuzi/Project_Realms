using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField]
    private int checkpointId;

    [SerializeField]
    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isActivated) return;

        if (other.CompareTag("Player"))
        {
            isActivated = true;
            GameManager.Instance.TriggerCheckpoint(checkpointId);
            OnCheckpointReached();
        }
    }

    private void OnCheckpointReached()
    {
        // 체크포인트 도달 시 시각적/청각적 피드백
        // 예: 파티클 효과, 사운드 등
    }

    public void ResetCheckpoint()
    {
        isActivated = false;
    }
}