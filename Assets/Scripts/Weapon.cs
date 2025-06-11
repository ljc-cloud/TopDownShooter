using System;
using UnityEngine.Serialization;

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

    public bool CanShoot()
    {
        return HaveEnoughBullets();
    }

    private bool HaveEnoughBullets()
    {
        if (bulletsInMagazine > 0)
        {
            bulletsInMagazine--;
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
}

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
