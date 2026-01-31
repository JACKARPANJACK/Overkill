using UnityEngine;
using System.Collections;

public class BossTurret : Destructible
{
    [Header("Boss Settings")]
    public float cycleDuration = 5f; // 5 seconds shield, 5 seconds attack
    public GameObject shieldVisual;  // The Blue Shield Sprite
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 0.5f;

    [Header("Ricochet Punishment")]
    public float reflectionDamage = 20f; // Damage to PLAYER if AI hits shield

    private bool isShieldActive = false;
    private float nextFireTime;
    private Transform player;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(CycleRoutine());
    }

    private void Update()
    {
        if (player == null || currentHealth <= 0) return;

        // ATTACK PHASE
        if (!isShieldActive)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    // The Cycle: Shield UP <-> Shield DOWN
    IEnumerator CycleRoutine()
    {
        while (true)
        {
            // Phase 1: Shield UP (Invulnerable)
            isShieldActive = true;
            shieldVisual.SetActive(true);
            yield return new WaitForSeconds(cycleDuration);

            // Phase 2: Shield DOWN (Vulnerable)
            isShieldActive = false;
            shieldVisual.SetActive(false);
            yield return new WaitForSeconds(cycleDuration);
        }
    }

    void Shoot()
    {
        // Simple Aim at Player
        Vector3 dir = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Instantiate(bulletPrefab, firePoint.position, transform.rotation);
    }

    // OVERRIDE DAMAGE: The Core Mechanic
    // If we take damage while shield is up, we hurt the PLAYER instead.
    public new void TakeDamage(float amount)
    {
        if (isShieldActive)
        {
            // PUNISHMENT!
            // We assume the AI shot the shield, so the Player takes the hit.
            IDamageable playerHealth = player.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                Debug.Log("RICOCHET! AI hit the shield, You took damage!");
                playerHealth.TakeDamage(reflectionDamage);
            }
        }
        else
        {
            // Standard Damage
            base.TakeDamage(amount);
        }
    }
}