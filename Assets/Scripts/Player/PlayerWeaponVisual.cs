using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;

public class PlayerWeaponVisual : MonoBehaviour
{
    private Animator _animator;
    private Player _player;
    

    [SerializeField] private WeaponModel[] weaponModelArray;
    [SerializeField] private BackUpWeaponModel[] backUpWeaponModelArray;
    
    [Header("Left Hand IK")]
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    private bool _shouldIncreaseLeftHandIKWeight;

    [Header("Rig")] [SerializeField] private float rigWeightIncreaseRate;
    private Rig _rig;
    private bool _shouldIncreaseRigWeight;

    private bool _isEquippingWeapon;
    private Transform _currentGun;
    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
        _rig = GetComponentInChildren<Rig>();
        weaponModelArray = GetComponentsInChildren<WeaponModel>(true);
        backUpWeaponModelArray = GetComponentsInChildren<BackUpWeaponModel>(true);
        SwitchOnCurrentWeaponModel();
    }

    private void Update()
    {
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

    public void PlayReloadAnimation()
    {
        ReduceRigWeight(0.15f);
        _animator.SetTrigger("Reload");
        Debug.Log("Reload");
    }

    #region Animation Rigging

    private void UpdateLeftHandIKWeight()
    {
        if (_shouldIncreaseLeftHandIKWeight)
        {
            leftHandIK.weight += leftHandIKWeightIncreaseRate * Time.deltaTime;
            if (leftHandIK.weight >= 1f)
            {
                _shouldIncreaseLeftHandIKWeight = false;
            }
        }
    }
    private void UpdateRigWeight()
    {
        if (_shouldIncreaseRigWeight)
        {
            _rig.weight += rigWeightIncreaseRate * Time.deltaTime;
            if (_rig.weight >= 1f)
            {
                _shouldIncreaseRigWeight = false;
            }
        }
    }
    private void ReduceRigWeight(float value) => _rig.weight = value;
    public void ReturnRigWeight() => _shouldIncreaseRigWeight = true;
    public void ReturnLeftHandIKWeight() => _shouldIncreaseLeftHandIKWeight = true;
    private void AttachLeftHand()
    {
        Transform targetTransform = GetCurrentWeaponModel().holdPoint;
        
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }
    
    #endregion
   
    public void SetBusyEquippingWeapon(bool busy)
    {
        _isEquippingWeapon = busy;
        _animator.SetBool("BusyEquippingWeapon", _isEquippingWeapon);
    }

    public WeaponModel GetCurrentWeaponModel()
    {
        WeaponModel currentWeaponModel = null;
        WeaponType currentWeaponType = _player.WeaponController.CurrentWeapon.weaponType;
        currentWeaponModel = weaponModelArray.First(item => item.weaponType == currentWeaponType);
        return currentWeaponModel;
    }

    public void PlayEquipWeaponAnimation()
    {
        WeaponModel currentWeaponModel = GetCurrentWeaponModel();
        WeaponEquipType equipType = currentWeaponModel.equipType;
        float equipSpeed = _player.WeaponController.CurrentWeapon.equipSpeed;
        
        leftHandIK.weight = 0f;
        ReduceRigWeight(0f);
        _animator.SetFloat("EquipSpeed", equipSpeed);
        _animator.SetFloat("WeaponEquipType", (float)equipType);
        _animator.SetTrigger("EquipWeapon");
        SetBusyEquippingWeapon(true);
    }

    public void SetCurrentWeaponAnimationParameter()
    {
        Weapon currentWeapon = _player.WeaponController.CurrentWeapon;
        _animator.SetFloat("EquipSpeed", currentWeapon.equipSpeed);
        _animator.SetFloat("ReloadSpeed", currentWeapon.reloadSpeed);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        SwitchOffWeaponModels();
        SwitchOffBackupWeaponModels();
        
        if (!_player.WeaponController.IsOnlyOneWeapon())
            SwitchOnBackupWeaponModel();
        
        WeaponModel currentWeaponModel = GetCurrentWeaponModel();
        currentWeaponModel.gameObject.SetActive(true);
        int currentWeaponAnimationLayer = (int)currentWeaponModel.holdType;
        SwitchAnimationLayer(currentWeaponAnimationLayer);
        AttachLeftHand();
    }
    private void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModelArray.Length; i++)
        {
            weaponModelArray[i].gameObject.SetActive(false);
        }
    }   
    private void SwitchOffBackupWeaponModels()
    {
        foreach (var backupWeaponModel in backUpWeaponModelArray)
        {
            backupWeaponModel.gameObject.SetActive(false);
        }
    }
    public void SwitchOnBackupWeaponModel()
    {
        WeaponType weaponType = _player.WeaponController.GetBackupWeapon().weaponType;
        foreach (var backUpWeaponModel in backUpWeaponModelArray)
        {
            if (weaponType == backUpWeaponModel.weaponType) 
                backUpWeaponModel.gameObject.SetActive(true);
        }
    }
    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }
        _animator.SetLayerWeight(layerIndex, 1);
    }
}