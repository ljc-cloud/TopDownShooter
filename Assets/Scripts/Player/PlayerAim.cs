using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerAim : MonoBehaviour
{
    private Player _player;
    private PlayerControls _controls;

    [Header("Aim Visual")] 
    [SerializeField] private LineRenderer aimLaser; 
    
    [Header("Aim Control")]
    [SerializeField] private Transform aimTarget;
    // 是否精确瞄准 - true: 枪口精准指向鼠标位置, false: 枪口指向与鼠标位置x,z所在平面的位置
    [SerializeField] private bool isAimPrecisely;
    // 是否锁定目标 - true: 鼠标进入Target的模型时，枪口指向锁定模型的中心位置, false: 枪口位置不锁定
    [SerializeField] private bool isLockingToTarget;
    
    [Header("Camera Control")]
    [SerializeField] private LayerMask aimLayerMask;
    [SerializeField] private Transform cameraTarget;
    [SerializeField, Range(0.5f, 1)] private float minCameraDistance;
    [SerializeField, Range(1, 3f)] private float maxCameraDistance;
    [SerializeField, Range(1, 5)] private float cameraSensitivity;
    private Vector3 _lookDirection;
    private Vector2 _mouseInput;
    private RaycastHit _lastMouseHit;
    
    public Transform AimTarget => aimTarget;
    public bool IsAimPrecisely => isAimPrecisely;

    private void Start()
    {
        _player = GetComponent<Player>();
        AssignInputEvents();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) isAimPrecisely = !isAimPrecisely;
        if (Input.GetKeyDown(KeyCode.L)) isLockingToTarget = !isLockingToTarget;

        UpdateAimVisual();
        UpdateAimPosition();
        UpdateCameraTargetPosition();
    }

    #region Aim Visual
    /// <summary>
    /// 更新枪口激光
    /// </summary>
    private void UpdateAimVisual()
    {
        aimLaser.enabled = _player.WeaponController.GetWeaponReady();
        if (!aimLaser.enabled) return;

        WeaponModel currentWeaponModel = _player.WeaponVisual.GetCurrentWeaponModel();
        
        currentWeaponModel.transform.LookAt(aimTarget);
        currentWeaponModel.gunPoint.LookAt(aimTarget);
        
        Transform gunPoint = currentWeaponModel.gunPoint;
        Vector3 laserDirection = _player.WeaponController.GetBulletDirection();
        float laserDistance = _player.WeaponController.CurrentWeapon.ShootDistance;
        float laserTipLength = .5f;
        
        aimLaser.SetPosition(0, gunPoint.position);
        Vector3 endPoint = gunPoint.position + laserDirection * laserDistance;
        if (Physics.Raycast(gunPoint.position, laserDirection, out RaycastHit hitInfo, laserDistance))
        {
            endPoint = hitInfo.point;
            laserTipLength = 0f;
        }
        
        aimLaser.SetPosition(1, endPoint);
        aimLaser.SetPosition(2, endPoint + laserDirection * laserTipLength);
    }
    /// <summary>
    /// 更新瞄准位置
    /// </summary>
    private void UpdateAimPosition()
    {
        Transform target = GetTarget();

        if (target is not null && isLockingToTarget)
        {
            // 避免瞄准向物体的枢轴点
            if (target.TryGetComponent(out Renderer ren))
            {
                aimTarget.position = ren.bounds.center;
            }
            else
            {
                aimTarget.position = target.position;
            }
            return;
        }

        aimTarget.position = GetMouseHit().point;
        
        if (!isAimPrecisely) 
            aimTarget.position = new Vector3(aimTarget.position.x
                , transform.position.y + 1, aimTarget.position.z);
    }
    #endregion

    #region Mouse Hit Info
    public Transform GetTarget()
    {
        Transform target = null;

        RaycastHit hitInfo = GetMouseHit();
        if (hitInfo.transform.GetComponent<Target>() != null)
        {
            target = hitInfo.transform;
        }
        return target;
    }
    
    public RaycastHit GetMouseHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(_mouseInput);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, aimLayerMask))
        {
            _lastMouseHit = hitInfo;
            return hitInfo;
        }
        return _lastMouseHit;
    }
    #endregion

    #region Camera
    
    private void UpdateCameraTargetPosition()
    {
        cameraTarget.position = Vector3.Lerp(cameraTarget.position, GetDesiredCameraPosition()
            , cameraSensitivity * Time.deltaTime);
    }
    private Vector3 GetDesiredCameraPosition()
    {
        bool movingDownwards = _player.Movement.MoveInput.y < -.5f;
        float actualMaxCameraDistance = movingDownwards ? minCameraDistance : maxCameraDistance;

        Vector3 desiredCameraPosition = GetMouseHit().point;
        Vector3 aimDirection = (desiredCameraPosition - cameraTarget.position).normalized;
        
        float distanceToDesiredAimPosition = Vector3.Distance(desiredCameraPosition, transform.position);
        float clampedDistance = Mathf.Clamp(distanceToDesiredAimPosition, minCameraDistance, actualMaxCameraDistance);
        desiredCameraPosition = transform.position + aimDirection * clampedDistance;
        desiredCameraPosition.y = transform.position.y + 1;
        
        return desiredCameraPosition;
    }

    #endregion

    #region Input Events
    private void AssignInputEvents()
    {
        _controls = _player.Controls;
        _controls.Character.Aim.performed += AimPerformed;
        _controls.Character.Aim.canceled += AimCanceled;
    }
    private void AimPerformed(InputAction.CallbackContext context)
    {
        _mouseInput = context.ReadValue<Vector2>();
    }
    private void AimCanceled(InputAction.CallbackContext context)
    {
        _mouseInput = Vector2.zero;
    }
    #endregion
}
