using System;
using UnityEngine;


public class PlayerAnimationEvents : MonoBehaviour
{
    private PlayerWeaponVisual _playerWeaponVisual;

    private void Awake()
    {
        _playerWeaponVisual = GetComponentInParent<PlayerWeaponVisual>();
    }

    public void ReloadOver()
    {
        _playerWeaponVisual.ReturnRigWeight();
    }

    public void ReturnRigWeight()
    {
        _playerWeaponVisual.ReturnRigWeight();
        _playerWeaponVisual.ReturnLeftHandIKWeight();
    }

    public void WeaponGrabOver()
    {
        _playerWeaponVisual.SetBusyGrabbingWeapon(false);
    }
}