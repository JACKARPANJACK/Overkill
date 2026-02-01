using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement, animation, and health logic.
/// Designed for a top-down 2D game using Unity's New Input System.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    /* =========================================================
     * MOVEMENT
     * ========================================================= */
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector2 movementInput;           // Raw input from keyboard / controller
    private Vector3 movementDirection;       // Normalized direction for logic
    private Rigidbody2D rb;

    /* =========================================================
     * HEALTH
     * ========================================================= */
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invulnerabilityDuration = 1f;

    private float currentHealth;
    private bool isInvulnerable;

    /* =========================================================
     * VISUALS
     * ========================================================= */
    [Header("Visual References")]
    [SerializeField] private Animator animator;

    private SpriteRenderer spriteRenderer;

    /* =========================================================
     * INPUT
     * ========================================================= */
    private PlayerInput inputActions;

    /* =========================================================
     * UNITY LIFECYCLE
     * ========================================================= */

    private RobotCompanion robo;
    public bool canDropAI = false;
    public bool inDropArea = false;
    private void Awake()
    {
        // Create instance of the auto-generated Input System class
        inputActions = new PlayerInput();
        if(robo ==null)
            robo = FindAnyObjectByType<RobotCompanion>();
        else
            Debug.Log("Robo not found");
    }

    private void OnEnable()
    {
        // Enable Player input map when object is active
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        // Disable input to prevent unwanted actions
        inputActions.Player.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Top-down game → no gravity

        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        /* -------------------- INPUT -------------------- */

        // Read movement input from Input System
        movementInput = inputActions.Player.Move.ReadValue<Vector2>();

        //Testing purpose: Drop AI companion
        if(Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (inDropArea) // player is in drop area
            {
                if (canDropAI)
                    ActivateRobo(false);
                else
                    ActivateRobo(true);
            }
        }
              
        

        // Normalize direction to avoid faster diagonal movement
        movementDirection = new Vector3(movementInput.x, movementInput.y, 0f).normalized;

        /* -------------------- ANIMATION -------------------- */

        animator.SetFloat("moveX", movementInput.x);
        animator.SetFloat("moveY", movementInput.y);
        animator.SetBool("moving", movementInput != Vector2.zero);

    }

    private void FixedUpdate()
    {
        // Apply movement using physics (best practice)
        rb.linearVelocity = movementInput * moveSpeed;
    }

    /* =========================================================
     * DAMAGE & HEALTH SYSTEM
     * ========================================================= */

    public void TakeDamage(float damage)
    {
        if (isInvulnerable) return;

        currentHealth -= damage;
        Debug.Log($"Player hit! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvulnerabilityFlash());
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");

        rb.linearVelocity = Vector2.zero;
        inputActions.Player.Disable();

        // Visual feedback for death
        if (spriteRenderer != null)
            spriteRenderer.color = Color.black;

        // Disable further logic
        enabled = false;
    }

    private IEnumerator InvulnerabilityFlash()
    {
        isInvulnerable = true;

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;

            // Brief red flash to indicate damage
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }

        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }


    public void ActivateRobo(bool activate)
    {
        if (robo != null)
            robo.isActive = activate;

        canDropAI = activate; 
    }


}
