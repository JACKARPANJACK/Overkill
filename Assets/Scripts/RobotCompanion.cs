using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Required for List
using UnityEngine.InputSystem; 

[RequireComponent(typeof(Rigidbody2D))
]
public class RobotCompanion : MonoBehaviour, PlayerInput.IPlayerActions
{
    private enum WeaponType { Minigun, Missile, Laser }

    [Header("Follow Settings")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float followDistance = 2.5f; 
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 8f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask viewBlockerMask;

    [Header("Visuals")]
    [SerializeField] private GameObject reticlePrefab; 
    [SerializeField] private Sprite validLockSprite;
    [SerializeField] private Sprite invalidLockSprite;
    
    // Single Active Reticle (The Locked Target)
    private GameObject activeReticle;
    private SpriteRenderer reticleRenderer;
    
    // Pool of Secondary Reticles (Potential Targets)
    private List<GameObject> secondaryReticles = new List<GameObject>();

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
    private PlayerInput inputActions;
    private Transform currentTarget;
    private List<Transform> allTargetsInRange = new List<Transform>(); // Cache of all targets
    private WeaponType currentWeapon = WeaponType.Minigun;
    private bool isAttackSuppressed = false;
    private bool manualLockOn = false; 

    private float nextMinigunTime;
    private float nextMissileTime;
    private float nextLaserTime;
    private bool isLaserActive;

    private void Awake()
    {
        inputActions = new PlayerInput();
        inputActions.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        if (laserRenderer != null) 
        {
            laserRenderer.enabled = false;
            laserRenderer.useWorldSpace = true; 
        }
        
        // Create Main Reticle
        if (reticlePrefab != null)
        {
            activeReticle = Instantiate(reticlePrefab, transform.position, Quaternion.identity);
            activeReticle.SetActive(false);
            reticleRenderer = activeReticle.GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        // 1. Scan logic
        if (!manualLockOn || currentTarget == null)
        {
            ScanForTargets(); // Updates currentTarget AND allTargetsInRange
        }

        // 2. Logic Interaction
        HandleCombat();
        UpdateAllReticles();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    // --- Core Logic ---

    private void UpdateAllReticles()
    {
        // 1. Update Main Reticle (Current Target)
        if (activeReticle != null)
        {
            if (currentTarget != null)
            {
                activeReticle.SetActive(true);
                activeReticle.transform.position = currentTarget.position;
                activeReticle.transform.Rotate(Vector3.forward * 200f * Time.deltaTime);
                
                if (reticleRenderer != null)
                {
                    bool canSee = HasLineOfSight(currentTarget);
                    Color targetColor = Color.yellow;
                    
                    if (isAttackSuppressed) targetColor = Color.gray;
                    else if (manualLockOn) targetColor = Color.red; 
                    if (!canSee) targetColor = new Color(0.5f, 0.5f, 0.5f, 0.5f); 

                    reticleRenderer.color = targetColor;
                    
                    // Main reticle is larger
                    activeReticle.transform.localScale = Vector3.one * 1.5f;

                    if (validLockSprite != null && invalidLockSprite != null)
                        reticleRenderer.sprite = canSee ? validLockSprite : invalidLockSprite;
                }
            }
            else
            {
                activeReticle.SetActive(false);
            }
        }

        // 2. Update Secondary Reticles (Other Targets)
        // Ensure pool is large enough
        while (secondaryReticles.Count < allTargetsInRange.Count)
        {
            GameObject newRet = Instantiate(reticlePrefab, transform.position, Quaternion.identity);
            var sr = newRet.GetComponent<SpriteRenderer>();
            if (sr) 
            {
                sr.color = new Color(1f, 1f, 1f, 0.3f); // Faint white
                sr.sortingOrder = -1; // Behind main
            }
            newRet.SetActive(false);
            secondaryReticles.Add(newRet);
        }

        // Position them
        for (int i = 0; i < secondaryReticles.Count; i++)
        {
            if (i < allTargetsInRange.Count)
            {
                Transform t = allTargetsInRange[i];
                // Don't draw secondary on the main target
                if (t != currentTarget && t != null)
                {
                    secondaryReticles[i].SetActive(true);
                    secondaryReticles[i].transform.position = t.position;
                    // Smaller and slower spin
                    secondaryReticles[i].transform.localScale = Vector3.one * 0.8f;
                    secondaryReticles[i].transform.Rotate(Vector3.forward * 50f * Time.deltaTime);
                }
                else
                {
                    secondaryReticles[i].SetActive(false);
                }
            }
            else
            {
                secondaryReticles[i].SetActive(false);
            }
        }
    }

    private void ScanForTargets()
    {
        allTargetsInRange.Clear();
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        
        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (var col in potentialTargets)
        {
            if (col.transform == transform) continue;

            IDamageable damageable = col.GetComponent<IDamageable>();
            if (damageable == null) damageable = col.GetComponentInParent<IDamageable>();
            if (damageable == null) damageable = col.GetComponentInChildren<IDamageable>();
            
            if (damageable != null)
            {
                allTargetsInRange.Add(col.transform); // Add to potential list

                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closest = col.transform;
                }
            }
        }

        currentTarget = closest;
    }
    
    // --- Input Callbacks ---
    public void OnMove(InputAction.CallbackContext context) { }
    
    public void OnSupression(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isAttackSuppressed = !isAttackSuppressed;
            UpdateAllReticles();
            Debug.Log($"[Robot] Attack Suppressed: {isAttackSuppressed}");
        }
    }

    public void OnLockon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (manualLockOn && currentTarget != null)
            {
                manualLockOn = false;
                Debug.Log("[Robot] Lock-On Disengaged");
            }
            else
            {
                ScanForTargets(); // Force fresh scan
                if (currentTarget != null)
                {
                    manualLockOn = true;
                    Debug.Log($"[Robot] Lock-On ENGAGED: {currentTarget.name}");
                }
            }
        }
    }

    public void OnChange_Weapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scrollValue = context.ReadValue<Vector2>().y;
            if (scrollValue > 0) currentWeapon = (WeaponType)(((int)currentWeapon + 1) % 3);
            else if (scrollValue < 0)
            {
                int prev = (int)currentWeapon - 1;
                if (prev < 0) prev = 2;
                currentWeapon = (WeaponType)prev;
            }
            Debug.Log($"[Robot] Changed Weapon to: {currentWeapon}");
        }
    }

    private bool HasLineOfSight(Transform target)
    {
        if (target == null) return false;
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, viewBlockerMask);
        return hit.collider == null;
    }

    private void HandleCombat()
    {
        if (isAttackSuppressed) return;
        if (currentTarget == null) return;
        if (!HasLineOfSight(currentTarget)) 
        {
            if (!manualLockOn) currentTarget = null;
            return;
        }

        switch (currentWeapon)
        {
            case WeaponType.Minigun:
                if (Time.time >= nextMinigunTime) { FireMinigun(); nextMinigunTime = Time.time + minigunFireRate; } break;
            case WeaponType.Missile:
                if (Time.time >= nextMissileTime) { FireMissile(); nextMissileTime = Time.time + missileFireRate; } break;
            case WeaponType.Laser:
                if (Time.time >= nextLaserTime && !isLaserActive) { StartCoroutine(FireChemicalLaser()); nextLaserTime = Time.time + laserCooldown; } break;
        }
    }

    private void FireMinigun()
    {
        if (minigunBulletPrefab == null || minigunBarrels == null || minigunBarrels.Length == 0) return;
        foreach (Transform barrel in minigunBarrels) 
            if (barrel != null) { GameObject bullet = Instantiate(minigunBulletPrefab, barrel.position, barrel.rotation); Physics2D.IgnoreCollision(GetComponent<Collider2D>(), bullet.GetComponent<Collider2D>()); }
    }

    private void FireMissile()
    {
         if (missilePrefab && missileBay)
        {
            GameObject missileObj = Instantiate(missilePrefab, missileBay.position, missileBay.rotation);
            Collider2D robotCol = GetComponent<Collider2D>(); Collider2D missCol = missileObj.GetComponent<Collider2D>();
            if(robotCol && missCol) Physics2D.IgnoreCollision(robotCol, missCol);
            if (currentTarget != null) missileObj.GetComponent<HomingMissile>()?.SetTarget(currentTarget);
        }
    }

    private IEnumerator FireChemicalLaser()
    {
        isLaserActive = true; laserRenderer.enabled = true; laserRenderer.SetPosition(0, laserOrigin.position);
        Vector2 laserDir = (currentTarget.position - laserOrigin.position).normalized;
        LayerMask combinedMask = enemyLayer | viewBlockerMask;
        RaycastHit2D hit = Physics2D.Raycast(laserOrigin.position, laserDir, attackRange, combinedMask);
        Vector3 endPoint;
        if (hit.collider != null)
        {
            endPoint = hit.point;
            if (((1 << hit.collider.gameObject.layer) & enemyLayer) != 0) 
            {
                 IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable == null) damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null) damageable.TakeDamage(laserDamage);
            }
        }
        else endPoint = laserOrigin.position + (Vector3)(laserDir * attackRange);
        laserRenderer.SetPosition(1, endPoint);
        yield return new WaitForSeconds(laserDuration);
        laserRenderer.enabled = false; isLaserActive = false;
    }

    private void HandleMovement()
    {
        if (playerTarget == null) return;
        float dist = Vector2.Distance(transform.position, playerTarget.position);
        if (dist > followDistance) rb.linearVelocity = (playerTarget.position - transform.position).normalized * moveSpeed;
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
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, followDistance);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}