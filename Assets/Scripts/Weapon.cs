using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Weapon
{
    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType weaponType;
    
    /// <summary>
    /// 武器当前弹容量
    /// </summary>
    public int bulletsInMagazine;
    /// <summary>
    /// 武器最大弹容量
    /// </summary>
    public int magazineCapacity;
    /// <summary>
    /// 全部剩余子弹数
    /// </summary>
    public int totalReserveAmmo;
    /// <summary>
    /// 装弹速度
    /// </summary>
    [Range(1, 5)] public float reloadSpeed = 1f;
    /// <summary>
    /// 装备速度
    /// </summary>
    [Range(1, 5)] public float equipSpeed = 1f;
    /// <summary>
    /// 开火速率（每秒可以发射多少子弹）
    /// </summary>
    [Space] public float fireRate = .8f;
    /// <summary>
    /// 默认开火速率
    /// </summary>
    public float defaultFireRate = .8f;
    /// <summary>
    /// 单发射击速率
    /// </summary>
    public float singleFireRate;
    /// <summary>
    /// 每次开火发射子弹数
    /// </summary>
    public int bulletsPerShot;
    
    /// <summary>
    /// 射击类型
    /// </summary>
    [Header("Fire Mode")]
    public FireType fireType;
    /// <summary>
    /// 是否可以单发射击
    /// </summary>
    public bool singleFireAvailable;
    /// <summary>
    /// 是否可以连发射击
    /// </summary>
    public bool burstAvailable;
    /// <summary>
    /// 是否可以自动射击
    /// </summary>
    public bool autoFireAvailable;
    
    /// <summary>
    /// 是否可以连发射击
    /// </summary>
    [Header("Burst Fire")] 
    public int burstModeBulletPerShots;
    /// <summary>
    /// 单个子弹发射间隔
    /// </summary>
    public float burstPerBulletFireInterval = .1f;
    /// <summary>
    /// 连发射击延迟
    /// </summary>
    public float burstFireDelay = .2f;
    /// <summary>
    /// 连发射击速率
    /// </summary>
    public float burstFireRate;
    
    /// <summary>
    /// 自动射击速率
    /// </summary>
    [Header("Auto Fire")]
    public float autoFireRate;
    
    /// <summary>
    /// 基础子弹散布
    /// </summary>
    [Header("Spread")] 
    [Range(0, 5)]
    public float baseSpread;
    /// <summary>
    /// 当前子弹散布
    /// </summary>
    public float currentSpread;
    /// <summary>
    /// 最大子弹散布
    /// </summary>
    public float maxSpread;
    /// <summary>
    /// 子弹散布速率
    /// </summary>
    public float spreadIncreaseRate;
    /// <summary>
    /// 子弹散布重置时间
    /// </summary>
    public float spreadCooldownTime;
    /// <summary>
    /// 上次子弹散布更新时间
    /// </summary>
    private float _lastUpdateSpreadTime;
    /// <summary>
    /// 上次开火时间
    /// </summary>
    private float _lastShootTime;
    
    /// <summary>
    /// 应用子弹散布
    /// </summary>
    /// <param name="originDirection">子弹初始方向</param>
    /// <returns>子弹加入散布的新方向</returns>
    public Vector3 ApplySpread(Vector3 originDirection)
    {
        UpdateSpread();
        float rdmRangeX = UnityEngine.Random.Range(-currentSpread, currentSpread);
        float rdmRangeY = UnityEngine.Random.Range(-currentSpread, currentSpread);
        float rdmRangeZ = UnityEngine.Random.Range(-currentSpread, currentSpread);

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
        if (Time.time > _lastUpdateSpreadTime + spreadCooldownTime)
        {
            currentSpread = baseSpread;
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
        currentSpread = Mathf.Clamp(currentSpread + spreadIncreaseRate, baseSpread, maxSpread);
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
        if (bulletsInMagazine == magazineCapacity) return false;
        if (totalReserveAmmo > 0)
        {
            return true;
        }
        return false;
    }
    public void ReloadBullets()
    {
        if (totalReserveAmmo <= 0) return;
        int needReloadBullets = magazineCapacity - bulletsInMagazine;
        needReloadBullets = needReloadBullets <= totalReserveAmmo ? needReloadBullets : totalReserveAmmo;
        bulletsInMagazine += needReloadBullets;
        totalReserveAmmo -=  needReloadBullets; 
        if (totalReserveAmmo < 0) totalReserveAmmo = 0;
    }

    public void SwitchFireMode()
    {
        FireType currentFireType = fireType;
        switch (currentFireType)
        {
            case FireType.Single:
                if (burstAvailable)
                    fireType = FireType.Burst;
                fireRate = burstFireRate; 
                break;
            case FireType.Burst:
                if (autoFireAvailable)
                    fireType = FireType.Auto;
                fireRate = autoFireRate;
                break;
            case FireType.Auto:
                if (singleFireAvailable)
                    fireType = FireType.Single;
                fireRate = singleFireRate;
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