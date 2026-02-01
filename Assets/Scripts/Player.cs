using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float invulnerabilityDuration = 1f;

    private float currentHealth;
    private bool isInvulnerable;
    private SpriteRenderer spriteRenderer;

    // Reference to the auto-generated C# class from the .inputactions file
    private PlayerInput inputActions;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    private RobotCompanion robo;
    public bool canDropAI = false;
    public bool inDropArea = false;
    private void Awake()
    {
        // Initialize the generated class
        inputActions = new PlayerInput();
        if(robo ==null)
            robo = FindAnyObjectByType<RobotCompanion>();
        else
            Debug.Log("Robo not found");
    }

    private void OnEnable()
    {
        // Enable the specific action map
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        // Disable the action map
        inputActions.Player.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ensure gravity doesn't pull the player down in a top-down view
        rb.gravityScale = 0f;

        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Read value from the "Move" action in the "Player" map
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
              
        
    }

    void FixedUpdate()
    {
        // Apply velocity in FixedUpdate for consistent physics behavior
        // Note: Using linearVelocity (Unity 6+) or velocity (older versions)
        rb.linearVelocity = movementInput * moveSpeed;
    }

    public void TakeDamage(float amount)
    {
        if (isInvulnerable) return;

        currentHealth -= amount;
        Debug.Log($"Player Hit! HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashInvulnerability());
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        rb.linearVelocity = Vector2.zero;
        
        // Disable Controls
        inputActions.Player.Disable();
        
        // Visual feedback for death
        if (spriteRenderer != null) 
            spriteRenderer.color = Color.black; // Turn black/dark to indicate death
            
        // Disable script to stop logic
        enabled = false;
    }

    private IEnumerator FlashInvulnerability()
    {
        isInvulnerable = true;
        Color originalColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        
        // Flash Red
        if (spriteRenderer != null) spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        
        // Restore
        if (spriteRenderer != null) spriteRenderer.color = originalColor;

        // Wait out the rest of the invulnerability
        yield return new WaitForSeconds(invulnerabilityDuration - 0.1f);
        isInvulnerable = false;
    }


    public void ActivateRobo(bool activate)
    {
        if (robo != null)
            robo.isActive = activate;

        canDropAI = activate; 
    }


}
