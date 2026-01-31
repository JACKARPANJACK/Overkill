// EnemyGunner.cs
using UnityEngine;

public class EGunner : EnemyBase
{
    public GameObject bulletPrefab;
    public Transform firePoint; // Assign a child object at the tip of the gun
    public float fireRate = 1.5f;
    private float nextFireTime;

    protected override void PerformAttack()
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}