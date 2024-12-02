using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponHolder : MonoBehaviour
{
    [Header("Rig Settings")]
    [SerializeField] private Transform rightHandTarget;
    [SerializeField] private Transform leftHandTarget;

    [Header("Weapon Settings")]
    [SerializeField] private float weaponScale = 100f;
    [SerializeField] private float ikWeightSpeed = 10f; // IK 전환 속도

    private GameObject currentWeaponObject;
    private Collider weaponCollider;
    private TrailRenderer weaponTrail;
    private ItemType currentWeaponType;

    public void EquipWeapon(GameObject weaponPrefab, ItemType weaponType)
    {
        UnequipCurrentWeapon();
        currentWeaponType = weaponType;

        if (weaponPrefab != null)
        {
            currentWeaponObject = Instantiate(weaponPrefab, rightHandTarget);
            currentWeaponObject.transform.localScale = Vector3.one * weaponScale;

            Transform weaponGrip = currentWeaponObject.transform.Find("GripPoint");
            if (weaponGrip != null && rightHandTarget != null)
            {

                if (weaponType == ItemType.Bow && leftHandTarget != null)
                {
                    Transform leftGrip = currentWeaponObject.transform.Find("LeftGripPoint");
                    if (leftGrip != null)
                    {
                        leftHandTarget.position = leftGrip.position;
                        leftHandTarget.rotation = leftGrip.rotation;
                    }
                }
            }

            weaponCollider = currentWeaponObject.GetComponentInChildren<Collider>();
            weaponTrail = currentWeaponObject.GetComponentInChildren<TrailRenderer>();
        }
    }

    public void UnequipCurrentWeapon()
    {
        if (currentWeaponObject != null)
        {
            Destroy(currentWeaponObject);
            currentWeaponObject = null;
            weaponCollider = null;
            weaponTrail = null;
            currentWeaponType = ItemType.None;
        }
    }

    public (Collider collider, TrailRenderer trail) GetWeaponComponents()
    {
        return (weaponCollider, weaponTrail);
    }
}