using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class RobotCompanion : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float followDistance = 2.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Visuals")]
    [SerializeField] private GameObject reticlePrefab;
    private GameObject activeReticle;

    [Header("Weapon: Minigun")]
    [SerializeField] private GameObject minigunBulletPrefab;
    [SerializeField] private Transform[] minigunBarrels;
    [SerializeField] private float minigunFireRate = 0.1f;

    [Header("Weapon: Homing Missiles")]
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform missileBay;
    [SerializeField] private float missileFireRate = 3f;

    [Header("Weapon: Chemical Laser")]
    [SerializeField] private LineRenderer laserRenderer;
    [SerializeField] private Transform laserOrigin; // Ensure this is assigned to the "Mouth" or laser port
    [SerializeField] private float laserDuration = 0.5f;
    [SerializeField] private float laserCooldown = 5f;
    [SerializeField] private float laserDamage = 50f;

    // Internal State
    private Rigidbody2D rb;
    private Transform currentTarget;
    private float nextMinigunTime;
    private float nextMissileTime;
    private float nextLaserTime;
    private bool isLaserActive;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (laserRenderer != null) 
        {
            laserRenderer.enabled = false;
            // Ensure world space is true so the line draws correctly in the world, not relative to robot rotation
            laserRenderer.useWorldSpace = true; 
        }
        
        if (reticlePrefab != null)
        {
            activeReticle = Instantiate(reticlePrefab, transform.position, Quaternion.identity);
            activeReticle.SetActive(false);
        }
    }

    private void Update()
    {
        FindClosestEnemy();
        HandleCombat();
        UpdateReticle();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void UpdateReticle()
    {
         if (activeReticle != null)
        {
            if (currentTarget != null)
            {
                activeReticle.SetActive(true);
                activeReticle.transform.position = currentTarget.position;
                activeReticle.transform.Rotate(Vector3.forward * 200f * Time.deltaTime);
            }
            else
            {
                activeReticle.SetActive(false);
            }
        }
    }

    private void FindClosestEnemy()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        
        float closestDistance = Mathf.Infinity;
        Transform closest = null;
        bool foundAnyEnemy = false;

        foreach (var col in potentialTargets)
        {
            if (col.transform == transform) continue;

            // Strict Search: Object -> Parent -> Children
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable == null) damageable = col.GetComponentInParent<IDamageable>();
            if (damageable == null) damageable = col.GetComponentInChildren<IDamageable>();
            
            if (damageable != null)
            {
                foundAnyEnemy = true;
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = col.transform;
                }
            }
        }

        currentTarget = closest;

        if (currentTarget != null)
        {
            // Debug: Green line to confirm lock
            Debug.DrawLine(transform.position, currentTarget.position, Color.green);
        }
    }

    private void HandleCombat()
    {
        if (currentTarget == null) return;

        // Minigun
        if (Time.time >= nextMinigunTime)
        {
            FireMinigun();
            nextMinigunTime = Time.time + minigunFireRate;
        }

        // Missiles
        if (Time.time >= nextMissileTime)
        {
            FireMissile();
            nextMissileTime = Time.time + missileFireRate;
        }

        // Laser
        if (Time.time >= nextLaserTime && !isLaserActive)
        {
            StartCoroutine(FireChemicalLaser());
            nextLaserTime = Time.time + laserCooldown;
        }
    }

    private void FireMinigun()
    {
        if (minigunBulletPrefab == null || minigunBarrels == null || minigunBarrels.Length == 0) return;

        foreach (Transform barrel in minigunBarrels) 
        {
            if (barrel != null) 
            {
                // Instantiate bullet at barrel position
                GameObject bullet = Instantiate(minigunBulletPrefab, barrel.position, barrel.rotation);
                
                // IMPORTANT: Ignore collision between bullet and robot to prevent self-damage or instant localized destruction
                Collider2D robotCollider = GetComponent<Collider2D>();
                Collider2D bulletCollider = bullet.GetComponent<Collider2D>();
                
                if (robotCollider != null && bulletCollider != null)
                {
                    Physics2D.IgnoreCollision(robotCollider, bulletCollider);
                }
            }
        }
    }

    private void FireMissile()
    {
         if (missilePrefab && missileBay)
        {
            GameObject missileObj = Instantiate(missilePrefab, missileBay.position, missileBay.rotation);
            
            // IMPORTANT: Ignore collision for missiles too
            Collider2D robotCollider = GetComponent<Collider2D>();
            Collider2D missileCollider = missileObj.GetComponent<Collider2D>();
            if (robotCollider != null && missileCollider != null)
            {
                Physics2D.IgnoreCollision(robotCollider, missileCollider);
            }

            if (currentTarget != null)
                missileObj.GetComponent<HomingMissile>()?.SetTarget(currentTarget);
        }
    }

    private IEnumerator FireChemicalLaser()
    {
        isLaserActive = true;
        laserRenderer.enabled = true;
        
        // --- COPIED & ADAPTED FROM ESniper.cs ---
        // 1. Set Start Position
        laserRenderer.SetPosition(0, laserOrigin.position);
        
        // 2. Raycast Logic
        // Calculate direction to currentTarget (better than using transform.up if the robot facing logic is laggy)
        Vector2 laserDir = (currentTarget.position - laserOrigin.position).normalized;
        
        // Perform Raycast
        RaycastHit2D hit = Physics2D.Raycast(laserOrigin.position, laserDir, attackRange, enemyLayer);
        
        Vector3 endPoint;

        // 3. Determine End Position
        if (hit.collider != null)
        {
            endPoint = hit.point;
            
            // Apply Damage
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable == null) damageable = hit.collider.GetComponentInParent<IDamageable>();
            
            if (damageable != null)
            {
                damageable.TakeDamage(laserDamage);
                Debug.Log($"Chemical Laser HIT: {hit.collider.name}");
            }
        }
        else
        {
            // If we missed or target moved, shoot straight for max range
            endPoint = laserOrigin.position + (Vector3)(laserDir * attackRange);
        }

        // 4. Set End Position
        laserRenderer.SetPosition(1, endPoint);

        yield return new WaitForSeconds(laserDuration);

        laserRenderer.enabled = false;
        isLaserActive = false;
    }

    private void HandleMovement()
    {
        if (playerTarget == null) return;
        float dist = Vector2.Distance(transform.position, playerTarget.position);
        if (dist > followDistance)
        {
            rb.linearVelocity = (playerTarget.position - transform.position).normalized * moveSpeed;
        }
        else rb.linearVelocity = Vector2.zero;
    }

    private void HandleRotation()
    {
         Vector2 lookTarget = currentTarget != null ? (Vector2)currentTarget.position : (rb.linearVelocity.magnitude > 0.1f ? rb.linearVelocity : Vector2.zero);
        if (lookTarget != Vector2.zero)
        {
            Vector2 dir = lookTarget - (Vector2)transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}