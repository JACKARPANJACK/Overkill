using UnityEngine;

public class HazardZone : MonoBehaviour
{
    [Header("Hazard Settings")]
    public float damagePerSecond = 10f;
    public float lifetime = 5f; // How long the fire lasts

    void Start()
    {
        // Automatically disappear after a few seconds
        Destroy(gameObject, lifetime);
    }

    // Runs every frame someone stands in the fire
    private void OnTriggerStay2D(Collider2D other)
    {
        // 1. Check if the object can take damage
        IDamageable target = other.GetComponent<IDamageable>();

        if (target != null)
        {
            // 2. Deal damage over time
            // We multiply by Time.deltaTime so it's "10 damage per second" 
            // instead of "10 damage per frame" (which would kill instantly)
            target.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}