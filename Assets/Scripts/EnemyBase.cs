using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : Destructible
{
    protected enum State { Guarding, Chasing, Returning, Attacking }
    [SerializeField] protected State currentState = State.Guarding;

    [Header("AI Settings")]
    public float detectionRadius = 8f;
    public float attackRange = 2f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;

    [Header("Death Settings")]
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionDamage = 40f;
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private LayerMask damageLayers;

    [Header("Vision Settings")]
    public LayerMask viewBlockerMask;
    public bool neverStopsChasing = false;

    protected Vector2 guardPosition;
    protected Transform player;
    protected Rigidbody2D rb;

    // 🔹 ADDED: Sprite reference for flipping
    protected SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // 🔹 ADDED: Cache sprite renderer (supports child sprites)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        guardPosition = transform.position;
    }

    protected override void OnHit()
    {
        if (currentState == State.Guarding || currentState == State.Returning)
        {
            currentState = State.Chasing;

            if (player != null)
            {
                SmoothLookAt(player.position);
                FaceTarget(player.position);   // 🔹 ADDED
            }
        }
    }

    protected override void Die()
    {
        if (explosionRadius > 0)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageLayers);
            foreach (var hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable == null) damageable = hit.GetComponentInParent<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(explosionDamage);
                }
            }
        }

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        base.Die();
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        bool hasClearLineOfSight = CheckLineOfSight(distToPlayer);

        switch (currentState)
        {
            case State.Guarding:
                rb.linearVelocity = Vector2.zero;

                if (distToPlayer < detectionRadius)
                {
                    SmoothLookAt(player.position);
                    FaceTarget(player.position);   // 🔹 ADDED

                    if (hasClearLineOfSight)
                        currentState = State.Chasing;
                }
                break;

            case State.Chasing:
                MoveTowards(player.position);

                if (distToPlayer <= attackRange && hasClearLineOfSight)
                    currentState = State.Attacking;
                else if (!hasClearLineOfSight && !neverStopsChasing)
                    currentState = State.Returning;
                break;

            case State.Attacking:
                rb.linearVelocity = Vector2.zero;

                SmoothLookAt(player.position);
                FaceTarget(player.position);       // 🔹 ADDED
                PerformAttack();

                if (distToPlayer > attackRange * 1.5f || !hasClearLineOfSight)
                    currentState = State.Chasing;
                break;

            case State.Returning:
                MoveTowards(guardPosition);

                if (distToPlayer < detectionRadius)
                {
                    SmoothLookAt(player.position);
                    FaceTarget(player.position);   // 🔹 ADDED

                    if (hasClearLineOfSight)
                        currentState = State.Chasing;
                }

                if (Vector2.Distance(transform.position, guardPosition) < 0.1f)
                    currentState = State.Guarding;
                break;
        }
    }

    protected void MoveTowards(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        SmoothLookAt(targetPos);
        FaceTarget(targetPos);   // 🔹 ADDED
    }

    // 🔹 ADDED: Sprite flip logic
    protected void FaceTarget(Vector2 targetPos)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.flipX = targetPos.x > transform.position.x;
    }

    protected void SmoothLookAt(Vector2 targetPos)
    {
        Vector2 direction = targetPos - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90;

        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    protected bool CheckLineOfSight(float distance)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, viewBlockerMask);
        return hit.collider == null;
    }

    protected abstract void PerformAttack();

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
