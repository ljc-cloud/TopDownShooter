using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject bulletHitVFX;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        CreateBulletVFX(other);
        
        _rb.velocity = Vector3.zero;
        Destroy(gameObject);
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
