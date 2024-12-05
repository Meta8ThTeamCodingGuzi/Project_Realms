using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSkill : Skill
{
    private bool isDash = true;
    private Player player;
    private Collider playerCollider;
    private Quaternion tarGetQuaternion;

    public override void Initialize()
    {
        base.Initialize();
        playerCollider = GetComponent<Collider>();
    }

    protected override void UseSkill()
    {
        if (isDash == false) return; 
        if (GetTargetDirection() == false) return; 
        StartCoroutine(DashRoutine());
    }



    private IEnumerator DashRoutine()
    {
        isDash = false;

        playerCollider.gameObject.SetActive(false);
        


        playerCollider.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        isDash = true;

        yield break;
    }


    private bool GetTargetDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPoint = ray.GetPoint(distance);
            Vector3 direction = (targetPoint - transform.position).normalized;
            direction.y = 0;
            tarGetQuaternion = Quaternion.LookRotation(direction);
            return true;
        }
        return false;
    }
}
