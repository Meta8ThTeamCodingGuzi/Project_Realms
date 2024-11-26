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

    }


    public void Update() //�� �����Ӹ��� Ȯ���ϴ� ������, ���� �κ��� �����𸣴ϱ��� 
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (InventoryOpen == false)
            {
                ShowInven();//�κ�UI�� ���� ������ ����
            }
            else
            {
                HideInven();//�ݴ� ������ ����
            }


        }

    }


    public void ShowInven()
    {
        inventoryUi.SetActive(true);
        InventoryOpen = true;
    }

    public void HideInven()
    {
        inventoryUi.SetActive(false);
        InventoryOpen = false;

    }


}
