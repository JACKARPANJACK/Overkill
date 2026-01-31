using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class 
    
    AI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float contactDamage = 10f;

    private Transform playerTarget;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Ensure top-down physics

        // Auto-find player if tagged "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    private void FixedUpdate()
    {
        if (playerTarget != null)
        {
            Vector2 direction = (playerTarget.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    // Deal damage if touching the player
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(contactDamage);
        }
    }
}