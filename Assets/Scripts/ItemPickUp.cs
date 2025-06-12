using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        other.GetComponent<PlayerWeaponController>()?.PickUpWeapon(weapon);
    }
}
