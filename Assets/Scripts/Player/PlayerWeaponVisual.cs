using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerWeaponVisual : MonoBehaviour
{
    private Animator _animator;
    
    #region Gun Transform
    [SerializeField] private Transform[] gunTransformArray;
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform revolver;
    [SerializeField] private Transform autoRifle;
    [SerializeField] private Transform shotgun;
    [SerializeField] private Transform sniperRifle;
    #endregion

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
        _rig = GetComponentInChildren<Rig>();
        SwitchOnGun(pistol);
    }

    private void Update()
    {
        CheckWeaponSwitch();

        if (Input.GetKeyDown(KeyCode.R) && !_isGrabbingWeapon)
        {
            ReduceRigWeight(0.15f);
            _animator.SetTrigger("Reload");
            Debug.Log("Reload");
        }
        
        UpdateRigWeight();
        UpdateLeftHandIKWeight();
    }

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

    private void ReduceRigWeight(float value)
    {
        _rig.weight = value;
    }

    public void ReturnRigWeight() => _shouldIncreaseRigWeight = true;
    public void ReturnLeftHandIKWeight() => _shouldIncreaseLeftHandIKWeight = true;

    public void SetBusyGrabbingWeapon(bool busy)
    {
        _isGrabbingWeapon = busy;
        _animator.SetBool("BusyGrabbingWeapon", _isGrabbingWeapon);
    }

    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchOnGun(pistol);
            SwitchAnimationLayer(1);
            PlayerGrabWeaponAnimation(WeaponGrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchOnGun(revolver);
            SwitchAnimationLayer(1);
            PlayerGrabWeaponAnimation(WeaponGrabType.SideGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchOnGun(autoRifle);
            SwitchAnimationLayer(1);
            PlayerGrabWeaponAnimation(WeaponGrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchOnGun(shotgun);
            SwitchAnimationLayer(2);
            PlayerGrabWeaponAnimation(WeaponGrabType.BackGrab);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchOnGun(sniperRifle);
            SwitchAnimationLayer(3);
            PlayerGrabWeaponAnimation(WeaponGrabType.BackGrab);
        }
    }

    private void PlayerGrabWeaponAnimation(WeaponGrabType grabType)
    {
        leftHandIK.weight = 0f;
        ReduceRigWeight(0f);
        _animator.SetFloat("WeaponGrabType", (float)grabType);
        _animator.SetTrigger("WeaponGrab");
        SetBusyGrabbingWeapon(true);
    }

    private void SwitchOnGun(Transform gunTransform)
    {
        SwitchOffGuns();
        gunTransform.gameObject.SetActive(true);
        _currentGun = gunTransform;
        AttachLeftHand();
    }

    private void SwitchOffGuns()
    {
        for (int i = 0; i < gunTransformArray.Length; i++)
        {
            gunTransformArray[i].gameObject.SetActive(false);
        }
    }

    private void AttachLeftHand()
    {
        Transform targetTransform = _currentGun.GetComponentInChildren<LeftHandTargetTransform>().transform;
        
        leftHandTarget.localPosition = targetTransform.localPosition;
        leftHandTarget.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 1; i < _animator.layerCount; i++)
        {
            _animator.SetLayerWeight(i, 0);
        }
        _animator.SetLayerWeight(layerIndex, 1);
    }

    public enum WeaponGrabType
    {
        SideGrab = 0,
        BackGrab = 1
    }
}