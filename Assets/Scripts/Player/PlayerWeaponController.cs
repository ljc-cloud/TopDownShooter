using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

// TODO: 完善对象池
public class PlayerWeaponController : MonoBehaviour
{
    // 标准子弹速度 刚体 mass = 1 —— 子弹速度20
    // 保证所有子弹速度都能一致的影响其他刚体
    private const float REFERENCE_BULLET_SPEED = 20f;

    private Player _player;
    [SerializeField] private Weapon currentWeapon;
    private bool _weaponReady = true;
    private bool _isShooting;

    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed;

    // [SerializeField] private Transform gunPoint;
    [SerializeField] private Transform weaponHolderTransform;

    [Header("Inventory")] [SerializeField] private List<Weapon> weaponSlots;
    [SerializeField] private int maxSlots = 2;

    private ObjectPool<GameObject> _bulletPool;

    // public Transform GunPoint => gunPoint;
    public Weapon CurrentWeapon => currentWeapon;

    private void Start()
    {
        _player = GetComponent<Player>();
        _bulletPool = new ObjectPool<GameObject>(() =>
        {
            Transform gunPoint = GetGunPoint();
            GameObject bulletGameObject = Instantiate(bulletPrefab, gunPoint.position,
                Quaternion.LookRotation(gunPoint.forward));
            bulletGameObject.SetActive(false);
            bulletGameObject.GetComponent<Bullet>().OnBulletHit += () =>
            {
                _bulletPool.Release(bulletGameObject);
            };
            return bulletGameObject;
        }, 
            actionOnGet: bullet => bullet.SetActive(true),
            actionOnRelease: bullet => bullet.SetActive(false), defaultCapacity: 10);

        AssignInputEvents();
        EquipWeapon(0);
    }

    private void Update()
    {
        if (_isShooting)
            Shoot();

        // 切换开火方式
        if (Input.GetKeyDown(KeyCode.B))
        {
            currentWeapon.SwitchFireMode();
        }
    }

    #region Input Events

    private void AssignInputEvents()
    {
        PlayerControls controls = _player.Controls;

        controls.Character.Fire.performed += _ => _isShooting = true;
        controls.Character.Fire.canceled += _ => _isShooting = false;
        controls.Character.EquipSlot1.performed += _ => EquipWeapon(0);
        controls.Character.EquipSlot2.performed += _ => EquipWeapon(1);
        controls.Character.DropCurrentWeapon.performed += DropWeapon;
        controls.Character.Reload.performed += _ => ReloadWeapon();
    }

    #endregion

    #region WeaponSlots Management

    private void EquipWeapon(int i)
    {
        if (i > weaponSlots.Count) return;
        if (!_weaponReady) return;
        SetWeaponReady(false);
        currentWeapon = weaponSlots[i];
        _player.WeaponVisual.ResetCurrentWeaponAnimationParameter();
        _player.WeaponVisual.PlayEquipWeaponAnimation();
    }

    private void DropWeapon(InputAction.CallbackContext _)
    {
        if (IsOnlyOneWeapon())
        {
            Debug.Log("can not drop weapon");
            return;
        }

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(weaponSlots.Count - 1);
    }

    public bool IsOnlyOneWeapon() => weaponSlots.Count <= 1;

    public void PickUpWeapon(Weapon newWeapon)
    {
        if (weaponSlots.Count >= maxSlots)
        {
            Debug.Log("can not pick up weapon");
            return;
        }

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
        for (int i = 1; i <= currentWeapon.burstModeBulletPerShots; i++)
        {
            SingleFire();
            yield return new WaitForSeconds(currentWeapon.burstPerBulletFireInterval);

            // if (i >= currentWeapon.bulletsPerShot) SetWeaponReady(true);
        }

        yield return new WaitForSeconds(currentWeapon.burstFireDelay);

        SetWeaponReady(true);
    }

    /// <summary>
    /// 单次射击
    /// </summary>
    private void SingleFire()
    {
        currentWeapon.bulletsInMagazine--;
        
        GameObject bulletGameObject = _bulletPool.Get();
        Transform gunPoint = GetGunPoint();
        bulletGameObject.transform.position = gunPoint.position;
        bulletGameObject.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
    
        bulletGameObject.SetActive(true);
        
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