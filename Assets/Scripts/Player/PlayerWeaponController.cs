using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    // 标准子弹速度 刚体 mass = 1 —— 子弹速度20
    // 保证所有子弹速度都能一致的影响其他刚体
    private const float REFERENCE_BULLET_SPEED = 20f;
    
    private Player _player;
    [SerializeField] private Weapon currentWeapon;
    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private Transform weaponHolderTransform;

    [Header("Inventory")] [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;
    
    public Transform GunPoint => gunPoint;
    public Weapon CurrentWeapon => currentWeapon;
    private void Start()
    {
        _player = GetComponent<Player>();
        AssignInputEvents();

        currentWeapon = weaponSlots[0];
        // currentWeapon.bulletsInMagazine = currentWeapon.magazineCapacity;
    }

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = _player.Controls;
        
        controls.Character.Fire.performed += FirePerformed;
        controls.Character.EquipSlot1.performed += _ => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += _ => EquipWeapon(1);
        controls.Character.DropCurrentWeapon.performed += DropWeaponPerformed;
        controls.Character.Reload.performed += _ =>
        {
            if (currentWeapon.CanReload())
            {
                _player.WeaponVisual.PlayReloadAnimation();
                // currentWeapon.ReloadBullets();
            }
        };
    }

    #endregion

    #region WeaponSlots Management

    private void EquipWeapon(int i)
    {
        currentWeapon = weaponSlots[i];
        _player.WeaponVisual.SwitchOffWeaponModels();
        _player.WeaponVisual.PlayEquipWeaponAnimation();
    }
    private void DropWeaponPerformed(InputAction.CallbackContext _)
    {
        if (weaponSlots.Count < 2)
        {
            Debug.Log("can not drop weapon");
            return;
        }

        weaponSlots.Remove(currentWeapon);
        currentWeapon = weaponSlots[^1];
    }
    public void PickUpWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("can not pick up weapon");
            return;
        }
        
        weaponSlots.Add(newWeapon);
    }

    #endregion

    private void FirePerformed(InputAction.CallbackContext context)
    {
        Shoot();
    }

    private void Shoot()
    {
        if (!currentWeapon.CanShoot()) return;
        
        GameObject bulletGameObject = Instantiate(bulletPrefab, gunPoint.position
            , Quaternion.LookRotation(gunPoint.forward));
        
        Rigidbody rb = bulletGameObject.GetComponent<Rigidbody>();
        rb.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rb.velocity = GetBulletDirection() * bulletSpeed;
        
        Destroy(bulletGameObject, 10f);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Vector3 GetBulletDirection()
    {
        Transform aimTargetTransform = _player.Aim.AimTarget;
        Vector3 bulletDirection = (aimTargetTransform.position - gunPoint.position).normalized;
        if (!_player.Aim.IsAimPrecisely && _player.Aim.GetTarget() is null)
            bulletDirection.y = 0;
        
        // TODO: find a better place
        // weaponHolderTransform.LookAt(aimTransform);
        // gunPoint.LookAt(aimTransform);
        
        return bulletDirection;
    }

    
}
