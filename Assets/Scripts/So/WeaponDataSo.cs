using UnityEngine;


[CreateAssetMenu(fileName = "WeaponData", menuName = "So/WeaponData")]
public class WeaponDataSo : ScriptableObject
{
    [Header("Base Weapon Data")]
    // public string weaponName;
    public WeaponType weaponType;
    [Tooltip("最大弹容量")] public int magazineCapacity;

    [Header("Speed")] [Tooltip("装弹速度"), Range(1, 5)]
    public float reloadSpeed = 1f;

    [Tooltip("装备速度"), Range(1, 5)] public float equipSpeed = 1f;


    // [Header("Bullets and magazine")]

    [Header("Fire Settings")]
    // public FireType fireType;
    [Tooltip("是否可以单发射击")] public bool singleFireAvailable;
    [Tooltip("是否可以连发")] public bool burstFireAvailable;
    [Tooltip("是否可以自动射击")] public bool autoFireAvailable;
    [Tooltip("射击速率")] public float fireRate;
    [Tooltip("单发模式射击速率")] public float singleFireRate;
    [Tooltip("自动模式射击速率")] public float autoFireRate;
    [Tooltip("每次射击子弹数")] public int bulletsPerShot;

    [Header("Burst Fire Settings")] 
    [Tooltip("连发模式射击速率")] public float burstFireRate;
    [Tooltip("连发模式每次射击子弹数")] public int burstFireBulletsPerShot;
    [Tooltip("连发模式每个子弹发射的间隔")] public float burstPerBulletFireInterval = .1f;
    [Tooltip("每次连发之间的间隔")] public float burstFireDelay = .2f;

    [Header("Spread")] 
    [Tooltip("基础散布"), Range(0, 5)] public float baseSpread;
    [Tooltip("最大散布")] public float maxSpread;
    [Tooltip("散布速率")] public float spreadIncreaseRate = .15f;
    [Tooltip("散布重置时间")] public float spreadCooldownTime;

    [Header("Distance")] [Tooltip("射击距离"), Range(2, 15)]
    public float shootDistance = 5f;

    [Tooltip("摄像机距离"), Range(4, 12)] public float cameraDistance;
}