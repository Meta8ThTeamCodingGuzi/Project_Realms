using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private int checkpointId;  // �� �踮� ���� üũ����Ʈ ID

    private void Start()
    {
        EnableBarrier();
    }

    public void DisableBarrier()
    {
        print("dsiable �踮�� ȣ��");
        gameObject.SetActive(false);
    }

    public void EnableBarrier()
    {
        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = true;
        }
    }

    public int CheckpointId => checkpointId;
}