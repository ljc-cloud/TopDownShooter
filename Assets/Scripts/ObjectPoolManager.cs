using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public const string BULLET = "bullet";
    public const string VFX = "impactFX";
    
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

    public void RegisterPool(string key, ObjectPool<GameObject> pool)
    {
        _poolDict.TryAdd(key, pool);
    }
    
    public ObjectPool<GameObject> GetPool(string key) => _poolDict.GetValueOrDefault(key, null);
    
    public bool HasPool(string key) => _poolDict.ContainsKey(key);
}
