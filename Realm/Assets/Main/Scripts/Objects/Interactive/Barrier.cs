using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private int checkpointId;  // 이 배리어가 속한 체크포인트 ID

    private void Start()
    {
        EnableBarrier();
    }

    public void DisableBarrier()
    {
        Destroy(this);
    }

    public void EnableBarrier()
    {
        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = true;
        }

        if (TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
        {
            renderer.enabled = true;
        }
    }

    public int CheckpointId => checkpointId;
}