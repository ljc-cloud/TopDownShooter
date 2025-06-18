using UnityEngine;

/// <summary>
/// 武器悬挂类型
/// </summary>
public enum WeaponHangType
{
    LowBack,
    Back,
    Side
}
public class BackUpWeaponModel : MonoBehaviour
{
    public WeaponType weaponType;
    [SerializeField] private WeaponHangType hangType;
    
    public void Activate(bool activate) => gameObject.SetActive(activate);
    
    public WeaponHangType HangType => hangType;
}
