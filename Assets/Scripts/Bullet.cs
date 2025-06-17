using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask bulletHitLayer;
    [SerializeField] private GameObject bulletHitVFX;
    private Rigidbody _rb;

    public event Action OnBulletHit;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        CreateBulletVFX(other);
        
        _rb.velocity = Vector3.zero;
        // Destroy(gameObject);
        int otherLayer = 1 << other.gameObject.layer;
        if ((bulletHitLayer.value & otherLayer) > 0)
        {
            OnBulletHit?.Invoke();
        }
    }

    private void CreateBulletVFX(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            ContactPoint contact = other.contacts[0];
            GameObject bulletImpact = Instantiate(bulletHitVFX, contact.point
                , Quaternion.LookRotation(contact.normal));
            Destroy(bulletImpact, 1f);
        }
    }
}
