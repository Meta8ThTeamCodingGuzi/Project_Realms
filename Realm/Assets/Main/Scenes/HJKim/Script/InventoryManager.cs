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
        InventoryOpen = false; //일단 기본은 닫혀있기

    }


    public void Update() //매 프레임마다 확인하는 이유는, 언제 인벤을 열지모르니까임 
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (InventoryOpen == false)
            {
                ShowInven();//인벤UI를 여는 동작을 담음
            }
            else
            {
                HideInven();//닫는 동작을 담음
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
