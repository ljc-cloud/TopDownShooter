using UnityEngine;

/// <summary>
/// 武器抓取类型
/// </summary>
public enum WeaponGrabType
{
    // 侧边抓取
    SideGrab = 0,
    // 背后抓取
    BackGrab = 1
}

/// <summary>
/// 武器握持类型（对应Animator中的layer）
/// </summary>
public enum WeaponHoldType
{
    Common = 1,
    Low,
    High
}
public class WeaponModel : MonoBehaviour
{
    /// <summary>
    /// 武器类型
    /// </summary>
    public WeaponType weaponType;
    /// <summary>
    /// 武器抓取类型
    /// </summary>
    public WeaponGrabType grabType;
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