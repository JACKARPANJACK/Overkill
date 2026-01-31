using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MinigunBullet : DamageDealer
{
    [SerializeField] private float speed = 20f;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        
        // Push bullet forward immediately upon spawn
        rb.linearVelocity = transform.up * speed;
    }

    private void Update()
    {
        // Align sprite rotation to velocity vector
        if (rb.linearVelocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}