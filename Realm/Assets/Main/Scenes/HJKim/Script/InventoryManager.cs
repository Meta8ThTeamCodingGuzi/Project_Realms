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
        Debug.Log("닫힘 완료!");
    }


    public void Update() //매 프레임마다 확인하는 이유는, 언제 인벤을 열지모르니까임 
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("I키가 눌리긴 합니다.");
            if (InventoryOpen == false)
            {
                ShowInven();//인벤UI를 여는 동작을 담음
            }
            else
            {
                HideInven();//닫는 동작을 담음
            }
            Debug.Log("열리거나 닫횜");

        }

    }


    public void ShowInven()
    {
        inventoryUi.SetActive(true);
        InventoryOpen = true;
        Debug.Log("우와 열렸어요");
    }

    public void HideInven()
    {
        inventoryUi.SetActive(false);
        InventoryOpen = false;
        Debug.Log("우와 닫혔어요!");
    }


}
