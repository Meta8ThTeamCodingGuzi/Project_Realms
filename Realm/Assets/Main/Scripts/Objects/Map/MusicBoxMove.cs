using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBoxMove : MonoBehaviour
{

    public float rotSpeed = 17f; // ������ �ӵ�

    void Update()
    {
        transform.Rotate(new Vector3(0, rotSpeed, 0) * Time.deltaTime);
    }
}
