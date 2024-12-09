using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PouBall : MonoBehaviour
{
    public float damage = 10f;  // ��������
    public float speed = 5f;  // �̵� �ӵ�
    public float lifeTime = 5f;  // ����ü ����
    private Vector3 direction;  // �̵� ����

    private void Start()
    {
        Destroy(gameObject, lifeTime);  // ������ ���ϸ� ����
    }

    // ��ǥ ��ġ�� �����ϰ� ���� ���
    public void SetTarget(Vector3 targetPosition)
    {
        direction = (targetPosition - transform.position).normalized;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;  // �̵�
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);  // ������ ������
            }
            Destroy(gameObject);  // ����ü ����
        }
    }




}
