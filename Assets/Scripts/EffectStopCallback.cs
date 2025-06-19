using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class EffectStopCallback : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _mainModule;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _mainModule = _particleSystem.main;
    }

    private void Start()
    {
        _mainModule.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        Debug.Log("Particle System Stopped");
        ObjectPoolManager.Instance.GetPool(ObjectPoolManager.VFX)?.Release(gameObject);
    }
}
