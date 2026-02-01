using UnityEngine;

public class Explosives : Destructible
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionDamage = 100f;

    [Header("Visuals")]
    public GameObject explosionVFX; // The Boom Particle
    public GameObject firePuddlePrefab; // The Oil Puddle (Leave empty for TNT)

    private void OnTriggerEnter2D(Collider2D other)
    {


        if (other.CompareTag("Player") && other.CompareTag("Bullet"))
        {
            Die();
        }
    }

    // Override the "Die" function from the Destructible script
    protected override void Die()
    {
        // 1. Spawn Visual Effect
        if (explosionVFX)
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

        // 2. Spawn Fire Puddle (Logic for Oil Barrels)
        if (firePuddlePrefab != null)
        {
            Instantiate(firePuddlePrefab, transform.position, Quaternion.identity);
        }

        // 3. Deal Area Damage
        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInRange)
        {
            // Don't damage ourselves (we are already dead)
            if (obj.gameObject == gameObject) continue;

            IDamageable target = obj.GetComponent<IDamageable>();
            if (target != null)
            {
                // This triggers Chain Reactions!
                target.TakeDamage(explosionDamage);
            }
        }

        // 4. Destroy the barrel object
        Destroy(gameObject);
    }

    // Visualize the radius in Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}