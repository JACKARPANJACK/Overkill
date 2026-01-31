using UnityEngine;

public class ESniper : EnemyBase
{
    [Header("Sniper Specifics")]
    public LineRenderer laserSight;
    public float chargeTime = 2.0f;
    public float damage = 50f;

    private float chargeTimer;

    protected override void Start()
    {
        base.Start();
        moveSpeed = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
    }

    protected override void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);
        bool canSee = CheckLineOfSight(distToPlayer);

        if (distToPlayer < detectionRadius && canSee)
        {
            SmoothLookAt(player.position);

            // If they are also close enough to shoot...
            if (distToPlayer <= attackRange)
            {
                PerformAttack(); // Start charging laser
            }
            else
            {
                ResetLaser(); // Player is seen, but too far away
            }
        }
        else
        {
            ResetLaser();
        }
    }

    protected override void PerformAttack()
    {
        laserSight.enabled = true;
        laserSight.SetPosition(0, transform.position);
        laserSight.SetPosition(1, player.position);

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeTime)
        {
            FireSniperShot();
            chargeTimer = 0; // Reset after firing
        }
    }

    private void ResetLaser()
    {
        laserSight.enabled = false;
        chargeTimer = 0;
    }

    void FireSniperShot()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 50f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
            {
                // Hit a wall, stop processing
                break;
            }

            // Check for Player
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("SNIPER HIT PLAYER");
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }
                break; // Stop ray after hitting player
            }
        }
        laserSight.enabled = false;
    }
}