using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private AreaData areaData;
    private Vector3 startPosition;//������ġ ��¿�
    private float distanceTraveled;//�̵���ġ Ȯ�ο�
    private Transform target;


    public void Initialize(AreaData Data)
    {
        this.areaData = Data;
        startPosition = transform.position;
    }


}
