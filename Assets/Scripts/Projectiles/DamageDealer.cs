using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected float lifeTime = 5f;
    [SerializeField] protected GameObject hitEffectPrefab;

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check direct component
        IDamageable damageable = other.GetComponent<IDamageable>();

        // 2. Check parent (For complex enemies made of multiple sprites)
        if (damageable == null)
            damageable = other.GetComponentInParent<IDamageable>();

        // 3. Apply damage if found
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // Spawn hit effect & Destroy regardless of hitting IDamageable 
        // (so bullets destroy on walls too)
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}