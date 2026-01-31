using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : Destructible
{
    protected enum State { Guarding, Chasing, Returning, Attacking }
    [SerializeField] protected State currentState = State.Guarding;

    [Header("AI Settings")]
    public float detectionRadius = 8f;   // The "Hearing" Range (Turns to look)
    public float attackRange = 2f;       // The "Firing" Range
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;     // How fast they turn (Smoothness)

    [Header("Vision Settings")]
    public LayerMask viewBlockerMask;    // Set to "Obstacles"
    public bool neverStopsChasing = false;

    protected Vector2 guardPosition;
    protected Transform player;
    protected Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        guardPosition = transform.position;
    }

    protected virtual void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        bool hasClearLineOfSight = CheckLineOfSight(distToPlayer);

        switch (currentState)
        {
            case State.Guarding:
                rb.linearVelocity = Vector2.zero; // Stop moving

                // BEHAVIOR: If player is close, turn to look at them!
                if (distToPlayer < detectionRadius)
                {
                    SmoothLookAt(player.position);

                    // If we can SEE them (no wall), start chasing
                    if (hasClearLineOfSight)
                    {
                        currentState = State.Chasing;
                    }
                }
                break;

            case State.Chasing:
                // Move towards player
                MoveTowards(player.position);

                // If in attack range AND we can see them -> Attack
                if (distToPlayer <= attackRange && hasClearLineOfSight)
                {
                    currentState = State.Attacking;
                }
                // If we lost them (Player hid behind wall) -> Go back?
                else if (!hasClearLineOfSight && !neverStopsChasing)
                {
                    currentState = State.Returning;
                }
                break;

            case State.Attacking:
                rb.linearVelocity = Vector2.zero;
                SmoothLookAt(player.position);
                PerformAttack();

                if (distToPlayer > attackRange * 1.5f || !hasClearLineOfSight)
                {
                    currentState = State.Chasing;
                }
                break;

            case State.Returning:
                MoveTowards(guardPosition);

                // While returning, if we hear/see player again, re-engage
                if (distToPlayer < detectionRadius)
                {
                    SmoothLookAt(player.position);
                    if (hasClearLineOfSight) currentState = State.Chasing;
                }

                if (Vector2.Distance(transform.position, guardPosition) < 0.1f)
                {
                    currentState = State.Guarding;
                }
                break;
        }
    }

    protected void MoveTowards(Vector2 targetPos)
    {
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        // Always look where you are going
        SmoothLookAt(targetPos);
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
        if (hit.collider != null)
        {
            return false;
        }
        return true;
    }

    protected abstract void PerformAttack();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}