using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    // 标准子弹速度 刚体 mass = 1 —— 子弹速度20
    // 保证所有子弹速度都能一致的影响其他刚体
    private const float REFERENCE_BULLET_SPEED = 20f;

    private Player _player;
    [SerializeField] private WeaponDataSo defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    private bool _weaponReady = true;
    private bool _isShooting;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform weaponHolderTransform;
    [SerializeField] private GameObject pickupWeaponPrefab;
    
    [Header("Inventory")]
    [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;
    
    public Weapon CurrentWeapon => currentWeapon;


    private void Start()
    {
        _player = GetComponent<Player>();
        
        AssignInputEvents();
        weaponSlots.Add(new Weapon(defaultWeaponData));
        EquipWeapon(0);
    }

    private void Update()
    {
        if (_isShooting)
            Shoot();
    }

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = _player.Controls;

        controls.Character.Fire.performed += _ => _isShooting = true;
        controls.Character.Fire.canceled += _ => _isShooting = false;
        controls.Character.EquipSlot1.performed += _ => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += _ => EquipWeapon(1);
        controls.Character.EquipSlot3.performed += _ => EquipWeapon(2);
        controls.Character.DropCurrentWeapon.performed += DropWeapon;
        controls.Character.Reload.performed += _ => ReloadWeapon();
        controls.Character.ChangeFireMode.performed += _ => currentWeapon.SwitchFireMode();
    }

    #endregion

    #region WeaponSlots Management

    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count) return;
        if (!_weaponReady) return;
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        _player.WeaponVisual.ResetCurrentWeaponAnimationParameter();
        _player.WeaponVisual.PlayEquipWeaponAnimation();
        
        CameraManager.Instance.ChangeCameraDistance(currentWeapon.CameraDistance);
    }

    private void DropWeapon(InputAction.CallbackContext _)
    {
        if (IsOnlyOneWeapon())
        {
            Debug.Log("can not drop weapon");
            return;
        }

        GameObject droppedWeaponGameObject = ObjectPoolManager.Instance.GetObject(ObjectPoolManager.PICKUP);
        droppedWeaponGameObject.GetComponent<PickupWeapon>()?.SetupPickupWeapon(currentWeapon, transform.position, transform.rotation);

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(weaponSlots.Count - 1);
    }

    public bool IsOnlyOneWeapon() => weaponSlots.Count <= 1;

    public void PickUpWeapon(Weapon newWeapon)
    {
        // 1. 如果武器槽中存在相同类型武器，将新武器的弹匣中的子弹添加到武器槽的武器中
        if (HasWeaponTypeInInventory(newWeapon.weaponType))
        {
            GetWeaponByType(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }
        
        // 2. 如果武器槽已满，且不存在与新武器类型相同的武器，丢弃武器槽中的武器，捡起新武器
        if (weaponSlots.Count >= maxSlots && currentWeapon.weaponType != newWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);
            _player.WeaponVisual.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;
            EquipWeapon(weaponIndex);
            return;
        }
        
        // 3. 武器槽未满，且不存在与新武器类型相同的武器
        weaponSlots.Add(newWeapon);
        _player.WeaponVisual.SwitchOnBackupWeaponModel();
    }

    public Weapon GetBackupWeapon()
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon != currentWeapon) return weapon;
        }

        return null;
    }

    public bool HasWeaponTypeInInventory(WeaponType weaponType)
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType) return true;
        }
        return false;
    }

    public Weapon GetWeaponByType(WeaponType weaponType)
    {
        foreach (var weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType) return weapon;
        }
        return null;
    }

    #endregion

    #region Weapon Control

    private void ReloadWeapon()
    {
        if (currentWeapon.CanReload() && _weaponReady)
        {
            SetWeaponReady(false);
            _player.WeaponVisual.PlayReloadAnimation();
            // currentWeapon.ReloadBullets();
        }
    }

    private void Shoot()
    {
        if (!currentWeapon.CanShoot() || !_weaponReady) return;

        _player.WeaponVisual.PlayShootAnimation();
        if (currentWeapon.fireType == FireType.Single)
        {
            _isShooting = false;
        }

        if (currentWeapon.fireType == FireType.Burst)
        {
            StartCoroutine(BurstFire());
            return;
        }

        SingleFire();
    }

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);
        for (int i = 1; i <= currentWeapon.BurstFireBulletPerShots; i++)
        {
            SingleFire();
            yield return new WaitForSeconds(currentWeapon.BurstPerBulletFireInterval);

            // if (i >= currentWeapon.bulletsPerShot) SetWeaponReady(true);
        }
        // yield return new WaitForSeconds(currentWeapon.BurstFireDelay);

        SetWeaponReady(true);
    }

    /// <summary>
    /// 单次射击
    /// </summary>
    private void SingleFire()
    {
        currentWeapon.bulletsInMagazine--;

        GameObject bulletGameObject = ObjectPoolManager.Instance.GetObject(ObjectPoolManager.BULLET);
        Transform gunPoint = GetGunPoint();
        bulletGameObject.transform.position = gunPoint.position;
        bulletGameObject.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
        
        Bullet bullet =  bulletGameObject.GetComponent<Bullet>();
        bullet.SetupBullet(currentWeapon.ShootDistance);

        Rigidbody rb = bulletGameObject.GetComponent<Rigidbody>();
        rb.mass = REFERENCE_BULLET_SPEED / bulletSpeed;

        Vector3 originDirection = GetBulletDirection();
        Vector3 bulletDirection = currentWeapon.ApplySpread(originDirection);
        rb.velocity = bulletDirection * bulletSpeed;
    }

    public Vector3 GetBulletDirection()
    {
        var gunPoint = GetGunPoint();
        Transform aimTargetTransform = _player.Aim.AimTarget;
        Vector3 bulletDirection = (aimTargetTransform.position - gunPoint.position).normalized;
        if (!_player.Aim.IsAimPrecisely && _player.Aim.GetTarget() is null)
            bulletDirection.y = 0;

        return bulletDirection;
    }

    private Transform GetGunPoint()
    {
        Transform gunPoint = _player.WeaponVisual.GetCurrentWeaponModel().gunPoint;
        return gunPoint;
    }

    #endregion

    #region Getter & Setter

    public void SetWeaponReady(bool ready) => _weaponReady = ready;
    public bool GetWeaponReady() => _weaponReady;

    #endregion
}