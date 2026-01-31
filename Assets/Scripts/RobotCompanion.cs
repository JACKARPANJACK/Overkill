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
    [SerializeField] private LayerMask viewBlockerMask; // NEW: Walls blind the robot

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
    [SerializeField] private Transform laserOrigin;
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
        FindClosestVisibleEnemy(); // Updated name to reflect logic
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

    private void FindClosestVisibleEnemy()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (var col in potentialTargets)
        {
            if (col.transform == transform) continue;

            // 1. Valid Target Check (IDamageable)
            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable == null) damageable = col.GetComponentInParent<IDamageable>();
            if (damageable == null) damageable = col.GetComponentInChildren<IDamageable>();
            
            if (damageable != null)
            {
                // 2. Line of Sight Check (Blinding Logic)
                if (HasLineOfSight(col.transform))
                {
                    float dist = Vector2.Distance(transform.position, col.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        closest = col.transform;
                    }
                }
            }
        }

        currentTarget = closest; // Only locks on if Visible

        if (currentTarget != null)
            Debug.DrawLine(transform.position, currentTarget.position, Color.green);
    }
    
    // NEW: Raycast check against walls
    private bool HasLineOfSight(Transform target)
    {
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        
        // Raycast against the Blocker Mask (Walls)
        // If we hit something closer than the enemy, we are blinded.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, viewBlockerMask);
        
        // If hit.collider is NOT null, it means we hit a wall before the enemy
        if (hit.collider != null)
        {
            // Debug failure (optional)
            // Debug.DrawRay(transform.position, direction.normalized * hit.distance, Color.yellow); 
            return false;
        }

        return true;
    }

    private void HandleCombat()
    {
        if (currentTarget == null) return;
        
        // Double check visibility right before firing (in case target ran behind wall during frame)
        if (!HasLineOfSight(currentTarget)) 
        {
            currentTarget = null;
            return;
        }

        if (Time.time >= nextMinigunTime)
        {
            FireMinigun();
            nextMinigunTime = Time.time + minigunFireRate;
        }

        if (Time.time >= nextMissileTime)
        {
            FireMissile();
            nextMissileTime = Time.time + missileFireRate;
        }

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
                GameObject bullet = Instantiate(minigunBulletPrefab, barrel.position, barrel.rotation);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>());
            }
        }
    }

    private void FireMissile()
    {
         if (missilePrefab && missileBay)
        {
            GameObject missileObj = Instantiate(missilePrefab, missileBay.position, missileBay.rotation);
            Collider2D robotCol = GetComponent<Collider2D>();
            Collider2D missCol = missileObj.GetComponent<Collider2D>();
            if(robotCol && missCol) Physics2D.IgnoreCollision(robotCol, missCol);

            if (currentTarget != null)
                missileObj.GetComponent<HomingMissile>()?.SetTarget(currentTarget);
        }
    }

    private IEnumerator FireChemicalLaser()
    {
        isLaserActive = true;
        laserRenderer.enabled = true;
        
        laserRenderer.SetPosition(0, laserOrigin.position);
        
        Vector2 laserDir = (currentTarget.position - laserOrigin.position).normalized;
        // Laser can be stopped by walls too
        LayerMask combinedMask = enemyLayer | viewBlockerMask;
        
        RaycastHit2D hit = Physics2D.Raycast(laserOrigin.position, laserDir, attackRange, combinedMask);
        
        Vector3 endPoint;

        if (hit.collider != null)
        {
            endPoint = hit.point;
            
            // Only damage if it's NOT a wall
            // (Check if layer is contained in enemyLayer mask)
            if (((1 << hit.collider.gameObject.layer) & enemyLayer) != 0) 
            {
                 IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable == null) damageable = hit.collider.GetComponentInParent<IDamageable>();
            
                if (damageable != null) damageable.TakeDamage(laserDamage);
            }
        }
        else
        {
            endPoint = laserOrigin.position + (Vector3)(laserDir * attackRange);
        }

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