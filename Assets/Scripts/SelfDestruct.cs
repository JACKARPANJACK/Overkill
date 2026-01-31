using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;

    void Start()
    {
        // Try to verify lifetime if a particle system is attached
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            // Destroy after the particle system finishes
            lifeTime = ps.main.duration + ps.main.startLifetime.constantMax;
        }

        Destroy(gameObject, lifeTime);
    }
}