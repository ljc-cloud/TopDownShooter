using System;
using UnityEngine;


public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisual _weaponVisual;
    private PlayerWeaponController _weaponController;

    private void Awake()
    {
        _weaponVisual = GetComponentInParent<PlayerWeaponVisual>();
        _weaponController = GetComponentInParent<PlayerWeaponController>();
    }

    public void ReloadOver()
    {
        _weaponVisual.ReturnRigWeight();
        _weaponController.CurrentWeapon.ReloadBullets();
    }

    public void ReturnRigWeight()
    {
        _weaponVisual.ReturnRigWeight();
        _weaponVisual.ReturnLeftHandIKWeight();
    }

    public void WeaponEquipOver()
    {
        _weaponVisual.SetBusyEquippingWeapon(false);
    }

    public void SwitchOnWeaponModel() => _weaponVisual.SwitchOnCurrentWeaponModel();
}