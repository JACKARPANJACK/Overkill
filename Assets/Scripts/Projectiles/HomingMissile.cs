using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HomingMissile : DamageDealer
{
    [Header("Movement")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float rotateSpeed = 200f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visuals")]
    [SerializeField] private LineRenderer trailRenderer; // Optional manual trail

    private Rigidbody2D rb;
    private Transform target;

    protected override void Start()
    {
        base.Start(); 
        rb = GetComponent<Rigidbody2D>();
        
        if (target == null) FindTarget();

        // Setup Trail if assigned (TrailRenderer is usually better, but LineRenderer requested)
        if (trailRenderer != null)
        {
            trailRenderer.positionCount = 0;
            trailRenderer.useWorldSpace = true;
        }
    }

    private void Update()
    {
        // Update Trail Visuals
        if (trailRenderer != null)
        {
            trailRenderer.positionCount++;
            trailRenderer.SetPosition(trailRenderer.positionCount - 1, transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            // Calculate direction to target
            Vector2 direction = (Vector2)target.position - rb.position;
            direction.Normalize();

            // Rotate towards target (cross product determines Left/Right turn)
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            rb.angularVelocity = -rotateAmount * rotateSpeed;
        }
        else
        {
            rb.angularVelocity = 0f;
             // Keep trying to find target
            FindTarget();
        }

        // Always move "forward" relative to current rotation
        rb.linearVelocity = transform.up * speed;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void FindTarget()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRadius, enemyLayer);
        // Find closest logic could go here, but first found is okay for dumb missiles
        if (hit != null) target = hit.transform;
    }
}