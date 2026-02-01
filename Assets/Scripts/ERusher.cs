// EnemyRusher.cs
using UnityEngine;

public class ERusher : EnemyBase
{
    public int damage = 10;
    public float attackRate = 1f;
    private float nextAttackTime;

    protected override void PerformAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            // Assume Player has a "PlayerHealth" script
            player.GetComponent<PlayerHealth>().TakeDamage(damage);
            Debug.Log("STAB!");
            nextAttackTime = Time.time + attackRate;
        }
    }
}