using System;

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
    public int magazineAmmo;
    /// <summary>
    /// 武器最大弹容量
    /// </summary>
    public int maxMagazineAmmo;

    public bool CanShoot()
    {
        return HaveEnoughBullets();
    }

    private bool HaveEnoughBullets()
    {
        if (magazineAmmo > 0)
        {
            magazineAmmo--;
            return true;
        }

        return false;
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
