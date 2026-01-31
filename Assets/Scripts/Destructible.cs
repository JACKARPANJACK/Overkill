// Destructible.cs
using UnityEngine;

public abstract class Destructible : MonoBehaviour, IDamageable
{
    [SerializeField] protected float maxHealth = 100f;
    protected float currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        OnHit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Simple 2D destroy
        Destroy(gameObject);
    }

    protected virtual void OnHit() { }
}