using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUi;
    public bool InventoryOpen;

    public void Start()
    {

        inventoryUi.SetActive(false);
        InventoryOpen = false; //�ϴ� �⺻�� �����ֱ�
        Debug.Log("���� �Ϸ�!");
    }


    public void Update() //�� �����Ӹ��� Ȯ���ϴ� ������, ���� �κ��� �����𸣴ϱ��� 
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("IŰ�� ������ �մϴ�.");
            if (InventoryOpen == false)
            {
                ShowInven();//�κ�UI�� ���� ������ ����
            }
            else
            {
                HideInven();//�ݴ� ������ ����
            }
            Debug.Log("�����ų� ��Ö");

        }

    }


    public void ShowInven()
    {
        inventoryUi.SetActive(true);
        InventoryOpen = true;
        Debug.Log("��� ���Ⱦ��");
    }

    public void HideInven()
    {
        inventoryUi.SetActive(false);
        InventoryOpen = false;
        Debug.Log("��� �������!");
    }


}
