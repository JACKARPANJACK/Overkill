// EnemySniper.cs
using UnityEngine;

public class ESniper : EnemyBase
{
    public LineRenderer laserSight;
    public float chargeTime = 2.0f;
    public float damage = 50f;
    private float chargeTimer;

    protected override void PerformAttack()
    {
        laserSight.enabled = true;
        laserSight.SetPosition(0, transform.position);
        laserSight.SetPosition(1, player.position);

        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeTime)
        {
            FireSniperShot();
            chargeTimer = 0;
        }
    }

    void FireSniperShot()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, 50f);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                Debug.Log("Sniper Hit Wall");
                break;
            }

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("SNIPER HIT PLAYER");
                IDamageable target = hit.collider.GetComponent<IDamageable>();
                if (target != null) target.TakeDamage(damage);
                break;
            }
        }

        laserSight.enabled = false;
    }

    // Disable laser when not attacking
    protected override void Update()
    {
        base.Update(); // Keep the movement logic!
        if (currentState != State.Attacking)
        {
            laserSight.enabled = false;
            chargeTimer = 0;
        }
    }
}