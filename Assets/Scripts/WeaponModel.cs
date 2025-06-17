using UnityEngine;

/// <summary>
/// 武器抓取类型
/// </summary>
public enum WeaponEquipType
{
    // 侧边抓取
    Side = 0,
    // 背后抓取
    Back = 1
}

/// <summary>
/// 武器握持类型（对应Animator中的layer）
/// </summary>
public enum WeaponHoldType
{
    // 持握普通高度
    Common = 1,
    // 持握低位
    Low,
    // 持握高位
    High
}
public class WeaponModel : MonoBehaviour
{
    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType weaponType;
    /// <summary>
    /// 武器装备类型
    /// </summary>
    public WeaponEquipType equipType;
    /// <summary>
    /// 武器左手持握类型
    /// </summary>
    public WeaponHoldType holdType;
    /// <summary>
    /// 枪口Trans
    /// </summary>
    public Transform gunPoint;
    /// <summary>
    /// 左手抓取点Trans 
    /// </summary>
    public Transform holdPoint;
}