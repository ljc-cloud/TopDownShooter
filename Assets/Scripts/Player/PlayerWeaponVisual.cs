using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Linq;

public class PlayerWeaponVisual : MonoBehaviour
{
    private Animator _animator;
    private Player _player;
    

    [SerializeField] private WeaponModel[] weaponModelArray;
    
    // [SerializeField] private Transform pistol;
    // [SerializeField] private Transform revolver;
    // [SerializeField] private Transform autoRifle;
    // [SerializeField] private Transform shotgun;
    // [SerializeField] private Transform sniperRifle;

    [Header("Left Hand IK")]
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private float leftHandIKWeightIncreaseRate;
    private bool _shouldIncreaseLeftHandIKWeight;

    [Header("Rig")] [SerializeField] private float rigWeightIncreaseRate;
    private Rig _rig;
    private bool _shouldIncreaseRigWeight;

    private bool _isGrabbingWeapon;
    private Transform _currentGun;
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _player = GetComponent<Player>();
        _rig = GetComponentInChildren<Rig>();
        weaponModelArray = GetComponentsInChildren<WeaponModel>(true);
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
   
    public void SetBusyGrabbingWeapon(bool busy)
    {
        _isGrabbingWeapon = busy;
        _animator.SetBool("BusyGrabbingWeapon", _isGrabbingWeapon);
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
        WeaponGrabType grabType = GetCurrentWeaponModel().grabType;
        leftHandIK.weight = 0f;
        ReduceRigWeight(0f);
        _animator.SetFloat("WeaponGrabType", (float)grabType);
        _animator.SetTrigger("WeaponGrab");
        SetBusyGrabbingWeapon(true);
    }

    public void SwitchOnCurrentWeaponModel()
    {
        // SwitchOffWeaponModels();
        WeaponModel currentWeaponModel = GetCurrentWeaponModel();
        currentWeaponModel.gameObject.SetActive(true);
        int currentWeaponAnimationLayer = (int)currentWeaponModel.holdType;
        SwitchAnimationLayer(currentWeaponAnimationLayer);
        AttachLeftHand();
    }

    public void SwitchOffWeaponModels()
    {
        for (int i = 0; i < weaponModelArray.Length; i++)
        {
            weaponModelArray[i].gameObject.SetActive(false);
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