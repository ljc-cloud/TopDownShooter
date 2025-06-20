using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// TODO: 弹药箱需要放入池中吗?
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletHitVFXPrefab;
    [SerializeField] private GameObject pickupWeaponPrefab;
    
    public const string BULLET = "bullet";
    public const string VFX = "impactFX";
    public const string PICKUP = "pickup";
    
    public static ObjectPoolManager Instance { get; private set; }
    public static Transform PoolParent { get; private set; }
    
    private readonly Dictionary<string, ObjectPool<GameObject>> _poolDict = new();
    
    private void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        PoolParent = transform;
    }

    private void Start()
    {
        ObjectPool<GameObject> bulletPool = new ObjectPool<GameObject>(() => Instantiate(bulletPrefab, PoolParent)
            , bulletGameObject => bulletGameObject.SetActive(true)
            , bulletGameObject => bulletGameObject.SetActive(false), defaultCapacity: 10);
        ObjectPool<GameObject> bulletHitPool = new ObjectPool<GameObject>(() => Instantiate(bulletHitVFXPrefab, PoolParent)
            , bulletGameObject => bulletGameObject.SetActive(true)
            , bulletGameObject => bulletGameObject.SetActive(false), defaultCapacity: 10);
        ObjectPool<GameObject> pickupWeaponPool = new ObjectPool<GameObject>(() => Instantiate(pickupWeaponPrefab, PoolParent)
            , bulletGameObject => bulletGameObject.SetActive(true)
            , bulletGameObject => bulletGameObject.SetActive(false), defaultCapacity: 10);
        
        _poolDict[BULLET] = bulletPool;
        _poolDict[VFX] = bulletHitPool;
        _poolDict[PICKUP] = pickupWeaponPool;
    }

    public void RegisterPool(string key, ObjectPool<GameObject> pool)
    {
        _poolDict.TryAdd(key, pool);
    }

    public void RegisterPool(string key, GameObject prefab, Action<GameObject> onGet, Action<GameObject> onRelease, Action<GameObject> onDestroy = null)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(()=> Instantiate(prefab), onGet, onRelease, onDestroy, defaultCapacity:10);
    }
    
    public ObjectPool<GameObject> GetPool(string key) => _poolDict.GetValueOrDefault(key, null);
    
    public bool HasPool(string key) => _poolDict.ContainsKey(key);

    public GameObject GetObject(string key)
    {
        if (_poolDict.TryGetValue(key, out var pool))
        {
            return pool.Get();
        }

        Debug.LogWarning($"pool {key} does not exist");
        return null;
    }

    public void ReleaseObject(string key, GameObject obj)
    {
        if (_poolDict.TryGetValue(key, out var pool))
        {
            pool.Release(obj);
        }
    }
}
