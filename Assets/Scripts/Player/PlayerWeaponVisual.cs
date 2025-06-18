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

    // private bool _isEquippingWeapon;
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

    #region Play Animation

    public void PlayShootAnimation() => _animator.SetTrigger("Fire");
    /// <summary>
    /// 播放装弹动画
    /// </summary>
    public void PlayReloadAnimation()
    {
        ReduceRigWeight(0.15f);
        _animator.SetTrigger("Reload");
        Debug.Log("Reload");
    }
    /// <summary>
    /// 播放装备武器动画
    /// </summary>
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
    }

    #endregion
    
    #region Animation Rigging
    /// <summary>
    /// 更新左手 IK权重，在装弹结束或装备结束后慢慢恢复权重
    /// 避免在装弹动画或装备动画时，左手IK权重过大，导致左手动作不正常（粘附在某一点上）
    /// </summary>
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
    /// <summary>
    /// 更新Rig 权重
    /// </summary>
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
    /// <summary>
    /// 减少Rig 权重
    /// </summary>
    /// <param name="value"></param>
    private void ReduceRigWeight(float value) => _rig.weight = value;
    /// <summary>
    /// 设置返回Rig权重标志位 true
    /// </summary>
    public void ReturnRigWeight() => _shouldIncreaseRigWeight = true;
    /// <summary>
    /// 设置返回左手IK权重标志位 true
    /// </summary>
    public void ReturnLeftHandIKWeight() => _shouldIncreaseLeftHandIKWeight = true;
    /// <summary>
    /// 设置左手IK目标Transform
    /// </summary>
    private void AttachLeftHand()
    {
        Transform targetTransform = GetCurrentWeaponModel().holdPoint;
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }
    #endregion

    #region Weapon Model Control

    /// <summary>
    /// 当前武器模型信息
    /// </summary>
    /// <returns></returns>
    public WeaponModel GetCurrentWeaponModel()
    {
        WeaponModel currentWeaponModel = null;
        WeaponType currentWeaponType = _player.WeaponController.CurrentWeapon.weaponType;
        currentWeaponModel = weaponModelArray.First(item => item.weaponType == currentWeaponType);
        return currentWeaponModel;
    }
    
    /// <summary>
    /// 重置当前武器的动画参数
    /// </summary>
    public void ResetCurrentWeaponAnimationParameter()
    {
        Weapon currentWeapon = _player.WeaponController.CurrentWeapon;
        _animator.SetFloat("EquipSpeed", currentWeapon.equipSpeed);
        _animator.SetFloat("ReloadSpeed", currentWeapon.reloadSpeed);
    }
    /// <summary>
    /// active当前武器模型
    /// </summary>
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
    /// <summary>
    /// 关闭所有武器模型
    /// </summary>
    private void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModelArray.Length; i++)
        {
            weaponModelArray[i].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 关闭备用武器模型
    /// </summary>
    private void SwitchOffBackupWeaponModels()
    {
        foreach (var backupWeaponModel in backUpWeaponModelArray)
        {
            backupWeaponModel.Activate(false);
        }
    }
    /// <summary>
    /// active备用武器模型
    /// </summary>
    public void SwitchOnBackupWeaponModel()
    {
        SwitchOffBackupWeaponModels();

        BackUpWeaponModel lowBackHangWeapon = null;
        BackUpWeaponModel backHangWeapon = null;
        BackUpWeaponModel sideHangWeapon = null;
        
        // WeaponType weaponType = _player.WeaponController.GetBackupWeapon().weaponType;
        foreach (var backUpWeaponModel in backUpWeaponModelArray)
        {
            if (_player.WeaponController.CurrentWeapon.weaponType == backUpWeaponModel.weaponType) 
                continue;
            if (_player.WeaponController.HasWeaponTypeInInventory(backUpWeaponModel.weaponType))
            {
                if (backUpWeaponModel.HangType == WeaponHangType.LowBack)
                    lowBackHangWeapon = backUpWeaponModel;

                if (backUpWeaponModel.HangType == WeaponHangType.Back)
                    backHangWeapon = backUpWeaponModel;

                if (backUpWeaponModel.HangType == WeaponHangType.Side)
                    sideHangWeapon = backUpWeaponModel;
            }
        }
        lowBackHangWeapon?.Activate(true);
        backHangWeapon?.Activate(true);
        sideHangWeapon?.Activate(true);
    }

    #endregion

    /// <summary>
    /// 设置是否正在装备武器
    /// </summary>
    /// <param name="busy"></param>
    // public void SetBusyEquippingWeapon(bool busy)
    // {
    //     _isEquippingWeapon = busy;
    //     _animator.SetBool("BusyEquippingWeapon", _isEquippingWeapon);
    // }
    /// <summary>
    /// 切换动画layer（针对不同类型武器的层级）
    /// </summary>
    /// <param name="layerIndex"></param>
    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }
        _animator.SetLayerWeight(layerIndex, 1);
    }
}