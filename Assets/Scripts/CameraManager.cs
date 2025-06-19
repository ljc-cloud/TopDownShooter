using System;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFramingTransposer _transposer;

    [SerializeField] private bool canChangeCameraDistance;
    [SerializeField] private float cameraDistanceChangeRate;
    private float _targetCameraDistance;

    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Have more than one instance of CameraManager");
            Destroy(gameObject);
        }
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        _transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void Update()
    {
        UpdateCameraDistance();
    }

    private void UpdateCameraDistance()
    {
        if (!canChangeCameraDistance) return;
        float currentDistance = _transposer.m_CameraDistance;
        if (Mathf.Abs(currentDistance - _targetCameraDistance) > 0.01f)
        {
            _transposer.m_CameraDistance = Mathf.Lerp(_transposer.m_CameraDistance, _targetCameraDistance, cameraDistanceChangeRate * Time.deltaTime);
        }
        else
        {
            _transposer.m_CameraDistance = _targetCameraDistance;
        }
    }

    public void ChangeCameraDistance(float distance) => _targetCameraDistance = distance;
}
