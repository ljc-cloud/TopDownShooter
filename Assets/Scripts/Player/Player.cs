using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerControls _controls;
    
    public PlayerControls Controls => _controls;
    public PlayerAim Aim { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerWeaponController WeaponController { get; private set; }
    public PlayerWeaponVisual WeaponVisual { get; private set; }

    private void Awake()
    {
        _controls = new PlayerControls();
        Aim = GetComponent<PlayerAim>();
        Movement = GetComponent<PlayerMovement>();
        WeaponController = GetComponent<PlayerWeaponController>();
        WeaponVisual = GetComponent<PlayerWeaponVisual>();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }
    private void OnDisable()
    {
        _controls.Disable();
    }
}