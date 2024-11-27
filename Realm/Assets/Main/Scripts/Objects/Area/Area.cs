using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private AreaData areaData;
    private Vector3 startPosition;//시작위치 담는용
    private float distanceTraveled;//이동위치 확인용
    private Transform target;


    public void Initialize(AreaData Data)
    {
        this.areaData = Data;
        startPosition = transform.position;
    }


}
