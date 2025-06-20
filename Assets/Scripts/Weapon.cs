using System;
using UnityEngine;

[Serializable]
public class Weapon
{
    #region Base Weapon Data

    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType weaponType;

    /// <summary>
    /// 射击类型
    /// </summary>
    public FireType fireType;

    /// <summary>
    /// 武器当前弹容量
    /// </summary>
    public int bulletsInMagazine;

    /// <summary>
    /// 武器最大弹容量
    /// </summary>
    public int MagazineCapacity { get; private set; }

    /// <summary>
    /// 全部剩余子弹数
    /// </summary>
    public int totalReserveAmmo;

    #endregion

    #region Fire Settings

    /// <summary>
    /// 是否可以单发射击
    /// </summary>
    public bool SingleFireAvailable { get; private set; }

    /// <summary>
    /// 是否可以连发射击
    /// </summary>
    public bool BurstFireAvailable { get; private set; }

    /// <summary>
    /// 是否可以自动射击
    /// </summary>
    public bool AutoFireAvailable { get; private set; }

    /// <summary>
    /// 开火速率（每秒可以发射多少子弹）
    /// </summary>
    public float fireRate;

    /// <summary>
    /// 单发射击速率
    /// </summary>
    public float SingleFireRate { get; private set; }

    /// <summary>
    /// 自动射击速率
    /// </summary>
    public float AutoFireRate { get; private set; }

    /// <summary>
    /// 每次开火发射子弹数
    /// </summary>
    public int BulletsPerShot { get; private set; }

    /// <summary>
    /// 默认开火速率
    /// </summary>
    public float DefaultFireRate { get; private set; }

    #endregion

    #region Burst Fire Settings

    /// <summary>
    /// 连发射击速率
    /// </summary>
    public float BurstFireRate { get; private set; }

    /// <summary>
    /// 连发模式每次射击子弹数
    /// </summary>
    public int BurstFireBulletPerShots { get; private set; }

    /// <summary>
    /// 单个子弹发射间隔
    /// </summary>
    public float BurstPerBulletFireInterval { get; private set; }

    /// <summary>
    /// 连发射击延迟
    /// </summary>
    public float BurstFireDelay { get; private set; }

    #endregion

    #region Spread

    /// <summary>
    /// 基础子弹散布
    /// </summary>
    public float BaseSpread { get; private set; }

    /// <summary>
    /// 当前子弹散布
    /// </summary>
    public float CurrentSpread { get; private set; }

    /// <summary>
    /// 最大子弹散布
    /// </summary>
    public float MaxSpread { get; private set; }

    /// <summary>
    /// 子弹散布速率
    /// </summary>
    public float SpreadIncreaseRate { get; private set; }

    /// <summary>
    /// 子弹散布重置时间
    /// </summary>
    public float SpreadCooldownTime { get; private set; }

    /// <summary>
    /// 上次子弹散布更新时间
    /// </summary>
    private float _lastUpdateSpreadTime;

    #endregion

    #region Speed

    /// <summary>
    /// 装弹速度
    /// </summary>
    public float ReloadSpeed { get; private set; }

    /// <summary>
    /// 装备速度
    /// </summary>
    public float EquipSpeed { get; private set; }

    #endregion

    #region Distance

    /// <summary>
    /// 射击距离
    /// </summary>
    public float ShootDistance { get; private set; }

    /// <summary>
    /// 摄像机距离
    /// </summary>
    public float CameraDistance { get; private set; }

    #endregion

    public WeaponDataSo WeaponDataSo { get; private set; }
    
    /// <summary>
    /// 上次开火时间
    /// </summary>
    private float _lastShootTime;

    public Weapon(WeaponDataSo weaponDataSoSo)
    {
        #region Specify

        weaponType = weaponDataSoSo.weaponType;
        MagazineCapacity = weaponDataSoSo.magazineCapacity;
        bulletsInMagazine = weaponDataSoSo.bulletsInMagazine;
        totalReserveAmmo = weaponDataSoSo.totalReserveAmmo;

        #endregion

        #region Spread

        BaseSpread = weaponDataSoSo.baseSpread;
        MaxSpread = weaponDataSoSo.maxSpread;
        SpreadIncreaseRate = weaponDataSoSo.spreadIncreaseRate;
        SpreadCooldownTime = weaponDataSoSo.spreadCooldownTime;

        #endregion

        #region Fire Settings

        SingleFireAvailable = weaponDataSoSo.singleFireAvailable;
        BurstFireAvailable = weaponDataSoSo.burstFireAvailable;
        AutoFireAvailable = weaponDataSoSo.autoFireAvailable;
        fireRate = weaponDataSoSo.fireRate;
        SingleFireRate = weaponDataSoSo.singleFireRate;
        AutoFireRate = weaponDataSoSo.autoFireRate;
        BulletsPerShot = weaponDataSoSo.bulletsPerShot;

        BurstFireRate = weaponDataSoSo.burstFireRate;
        BurstFireBulletPerShots = weaponDataSoSo.burstFireBulletsPerShot;
        BurstPerBulletFireInterval = weaponDataSoSo.burstPerBulletFireInterval;
        BurstFireDelay = weaponDataSoSo.burstFireDelay;

        #endregion

        #region Speed

        ReloadSpeed = weaponDataSoSo.reloadSpeed;
        EquipSpeed = weaponDataSoSo.equipSpeed;

        #endregion

        #region Distance

        ShootDistance = weaponDataSoSo.shootDistance;
        CameraDistance = weaponDataSoSo.cameraDistance;

        #endregion

        WeaponDataSo = weaponDataSoSo;
    }

    /// <summary>
    /// 应用子弹散布
    /// </summary>
    /// <param name="originDirection">子弹初始方向</param>
    /// <returns>子弹加入散布的新方向</returns>
    public Vector3 ApplySpread(Vector3 originDirection)
    {
        UpdateSpread();
        float rdmRangeX = UnityEngine.Random.Range(-CurrentSpread, CurrentSpread);
        float rdmRangeY = UnityEngine.Random.Range(-CurrentSpread, CurrentSpread);
        float rdmRangeZ = UnityEngine.Random.Range(-CurrentSpread, CurrentSpread);

        Quaternion spreadRotation = Quaternion.Euler(rdmRangeX, rdmRangeY, rdmRangeZ);
        // Vector3 newDirection = originDirection + new Vector3(rdmRangeX, 0, rdmRangeZ);
        return spreadRotation * originDirection;
    }

    /// <summary>
    /// 更新子弹散布
    /// 超过冷却时间，重置子弹散布
    /// </summary>
    private void UpdateSpread()
    {
        if (Time.time > _lastUpdateSpreadTime + SpreadCooldownTime)
        {
            CurrentSpread = BaseSpread;
        }
        else
        {
            IncreaseSpread();
        }

        _lastUpdateSpreadTime = Time.time;
    }

    /// <summary>
    /// 随机增加子弹散布
    /// </summary>
    private void IncreaseSpread()
    {
        CurrentSpread = Mathf.Clamp(CurrentSpread + SpreadIncreaseRate, BaseSpread, MaxSpread);
    }

    public bool CanShoot() => HaveEnoughBullets() && ReadyToFire();
    private bool HaveEnoughBullets() => bulletsInMagazine > 0;

    private bool ReadyToFire()
    {
        if (Time.time > _lastShootTime + 1 / fireRate)
        {
            _lastShootTime = Time.time;
            return true;
        }

        return false;
    }

    public bool CanReload()
    {
        if (bulletsInMagazine == MagazineCapacity) return false;
        if (totalReserveAmmo > 0)
        {
            return true;
        }

        return false;
    }

    public void ReloadBullets()
    {
        if (totalReserveAmmo <= 0) return;
        int needReloadBullets = MagazineCapacity - bulletsInMagazine;
        needReloadBullets = needReloadBullets <= totalReserveAmmo ? needReloadBullets : totalReserveAmmo;
        bulletsInMagazine += needReloadBullets;
        totalReserveAmmo -= needReloadBullets;
        if (totalReserveAmmo < 0) totalReserveAmmo = 0;
    }

    public void SwitchFireMode()
    {
        FireType currentFireType = fireType;
        switch (currentFireType)
        {
            case FireType.Single:
                if (BurstFireAvailable)
                    fireType = FireType.Burst;
                fireRate = BurstFireRate;
                break;
            case FireType.Burst:
                if (AutoFireAvailable)
                    fireType = FireType.Auto;
                fireRate = AutoFireRate;
                break;
            case FireType.Auto:
                if (SingleFireAvailable)
                    fireType = FireType.Single;
                fireRate = SingleFireRate;
                break;
            default: break;
        }
    }
}

/// <summary>
/// 武器类型
/// </summary>
public enum WeaponType
{
    // 手枪
    Pistol,

    // 左轮
    Revolver,

    // 自动步枪
    AutoRifle,

    // 霰弹枪
    Shotgun,

    // 狙击枪
    SniperRifle,
}

/// <summary>
/// 武器射击类型
/// </summary>
public enum FireType
{
    Single,
    Burst,
    Auto
}