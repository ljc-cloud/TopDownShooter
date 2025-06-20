using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弹药箱枚举
/// </summary>
public enum AmmoBoxType
{
    // 轻型
    Light,
    // 重型
    Heavy
}

[System.Serializable]
public struct AmmoData
{
    public WeaponType weaponType;
    [Range(10, 100)] public int minAmount;
    [Range(10, 100)] public int maxAmount;
}
public class PickupAmmo : Interactable, IPickup
{
    [SerializeField] private AmmoBoxType ammoBoxType;
    [SerializeField] private List<AmmoData> lightAmmoData; 
    [SerializeField] private List<AmmoData> heavyAmmoData;
    [SerializeField] private GameObject[] boxModelArray;

    private PlayerWeaponController _weaponController;
    
    private void Start()
    {
        SetupBoxModel();
    }

    private void SetupBoxModel()
    {
        for (int i = 0; i < boxModelArray.Length; i++)
        {
            if (i == (int)ammoBoxType)
            {
                boxModelArray[i].SetActive(true);
                UpdateMeshRendererAndMaterial(boxModelArray[i].GetComponent<MeshRenderer>());
            }
            else
            {
                boxModelArray[i].SetActive(false);
            }
        }
    }
    
    public override void Interact()
    {
        Debug.Log("Pickup ammo");
        List<AmmoData> currentAmmoData = ammoBoxType == AmmoBoxType.Light ? lightAmmoData : heavyAmmoData;

        foreach (var ammoData in currentAmmoData)
        {
            Weapon weapon = _weaponController.GetWeaponByType(ammoData.weaponType);
            AddBulletsToWeapon(weapon, GetRandomAmount(ammoData));
        }
    }

    private int GetRandomAmount(AmmoData ammoData)
    {
        int min = Mathf.Min(ammoData.minAmount, ammoData.maxAmount);
        int max = Mathf.Max(ammoData.minAmount, ammoData.maxAmount);
        
        return UnityEngine.Random.Range(min, max + 1);
    }

    private void AddBulletsToWeapon(Weapon weapon, int amount)
    {
        if (weapon != null)
        {
            weapon.totalReserveAmmo += amount;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        _weaponController ??= other.GetComponent<PlayerWeaponController>();
    }
}
