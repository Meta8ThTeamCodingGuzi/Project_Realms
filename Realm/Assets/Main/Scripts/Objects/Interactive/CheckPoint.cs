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
        // üũ����Ʈ ���� �� �ð���/û���� �ǵ��
        // ��: ��ƼŬ ȿ��, ���� ��
    }

    public void ResetCheckpoint()
    {
        isActivated = false;
    }
}