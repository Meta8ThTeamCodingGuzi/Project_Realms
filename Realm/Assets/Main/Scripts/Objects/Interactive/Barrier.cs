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
        gameObject.SetActive(true);
    }

    public int CheckpointId => checkpointId;
}