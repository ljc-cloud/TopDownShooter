using System;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask bulletHitLayer;
    [SerializeField] private GameObject bulletHitVFX;
    
    private Rigidbody _rb;
    private TrailRenderer _trailRenderer;
    private BoxCollider _collider;
    private MeshRenderer _meshRenderer;
    
    private Vector3 _startPosition;
    private float _flyDistance;
    private bool _bulletDisabled;

    // public event Action<GameObject> OnReleaseBullet;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _trailRenderer = GetComponent<TrailRenderer>();
        _collider = GetComponent<BoxCollider>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetupBullet(float flyDistance)
    {
        _collider.enabled = true;
        _meshRenderer.enabled = true;
        _bulletDisabled = false;
        _startPosition = transform.position;
        _flyDistance = flyDistance + .5f; // 。5f 激光尖端长度
        _trailRenderer.time = .25f;
    }

    private void Update()
    {
        FadeBulletTrail();
        DisableBulletIfNeeded();
        ReleaseBulletIfNeeded();
    }

    private void ReleaseBulletIfNeeded()
    {
        if (_trailRenderer.time <= 0)
        {
            if (gameObject.activeSelf)
                ObjectPoolManager.Instance.GetPool(ObjectPoolManager.BULLET)?.Release(gameObject);
        }
    }

    private void DisableBulletIfNeeded()
    {
        if (Vector3.Distance(transform.position, _startPosition) >= _flyDistance & !_bulletDisabled)
        {
            _collider.enabled = false;
            _meshRenderer.enabled = false;
            _bulletDisabled = true;
        }
    }

    private void FadeBulletTrail()
    {
        if (Vector3.Distance(transform.position, _startPosition) >= _flyDistance - 1.5f)
        {
            _trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        CreateBulletVFX(other);
        
        _rb.velocity = Vector3.zero;
        // Destroy(gameObject);
        int otherLayer = 1 << other.gameObject.layer;
        if ((bulletHitLayer.value & otherLayer) > 0)
        {
            if (gameObject.activeSelf)
                ObjectPoolManager.Instance.GetPool(ObjectPoolManager.BULLET)?.Release(gameObject);
        }
    }

    private void CreateBulletVFX(Collision other)
    {
        ObjectPool<GameObject> impactPool;
        if (!ObjectPoolManager.Instance.HasPool(ObjectPoolManager.VFX))
        {
            impactPool = new ObjectPool<GameObject>(() => Instantiate(bulletHitVFX, ObjectPoolManager.PoolParent),
                actionOnGet: impact => impact.SetActive(true),
                actionOnRelease: impact => impact.SetActive(false),
                defaultCapacity: 10);
            ObjectPoolManager.Instance.RegisterPool(ObjectPoolManager.VFX, impactPool);
        }
        else
        {
            impactPool = ObjectPoolManager.Instance.GetPool(ObjectPoolManager.VFX);
        }
        if (other.contacts.Length > 0)
        {
            GameObject impactGameObject = impactPool.Get();
            impactGameObject.transform.position = other.contacts[0].point;
            impactGameObject.transform.rotation = Quaternion.LookRotation(other.contacts[0].normal);
        }
    }
}
