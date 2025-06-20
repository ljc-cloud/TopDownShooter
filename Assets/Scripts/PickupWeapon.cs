using System;
using UnityEngine;

public class PickupWeapon : Interactable, IPickup
{
    [SerializeField] private WeaponDataSo weaponDataSo; 
    [SerializeField] private PickupWeaponModel[] modelArray;

    private PlayerWeaponController _weaponController;
    private Weapon _weapon;
    private bool _oldWeapon;

    private void Start()
    {
        if (!_oldWeapon)
            _weapon = new Weapon(weaponDataSo);
        UpdateGameObject();
    }

    public void SetupPickupWeapon(Weapon weapon, Vector3 position, Quaternion rotation)
    {
        _oldWeapon = true;
            
        _weapon = weapon;
        weaponDataSo = weapon.WeaponDataSo;
        transform.position = position + Vector3.up * .2f;
        transform.rotation = rotation;
        UpdateGameObject();
    }

    [ContextMenu("Update GameObject")]
    public void UpdateGameObject()
    {
        gameObject.name = $"Pickup Weapon-{weaponDataSo.weaponType}";
        UpdateWeaponModels();
    }
    private void UpdateWeaponModels()
    {
        foreach (var model in modelArray)
        {
            model.gameObject.SetActive(false);
            if (model.weaponType == weaponDataSo.weaponType)
            {
                model.gameObject.SetActive(true);
                UpdateMeshRendererAndMaterial(model.meshRenderer);
            }
        }
    }
    
    public override void Interact()
    {
        Debug.Log($"Pickup weapon: {weaponDataSo.weaponType}");
        _weaponController?.PickUpWeapon(_weapon);
     
        ObjectPoolManager.Instance.ReleaseObject(ObjectPoolManager.PICKUP, gameObject);
        
        // Destroy(gameObject);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        _weaponController ??= other.transform.GetComponent<PlayerWeaponController>();
    }
}
