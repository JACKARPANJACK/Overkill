// ExplosiveBarrel.cs
using UnityEngine;

public class Barrel : Destructible
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionDamage = 100f;
    public GameObject explosionVFX;

    protected override void Die()
    {
        if (explosionVFX) Instantiate(explosionVFX, transform.position, Quaternion.identity);

        // 2D Overlap check
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D nearbyObject in colliders)
        {
            IDamageable target = nearbyObject.GetComponent<IDamageable>();

            // Avoid infinite loops (don't damage self)
            if (target != null && target != (IDamageable)this)
            {
                target.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}