using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    
    // Helper events for UI updates or Effects (blood particles, sounds)
    [Header("Events")]
    public UnityEvent<float> OnDamageTaken;
    public UnityEvent OnDeath;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        
        // Notify listeners (UI bars, damage numbers)
        OnDamageTaken?.Invoke(amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Notify listeners (Score managers, Quest systems)
        OnDeath?.Invoke();
        
        // Simple destruction, can be replaced by object pooling or death animations
        Destroy(gameObject);
    }
}