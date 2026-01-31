using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    // Reference to the auto-generated C# class from the .inputactions file
    private PlayerInput inputActions;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    private void Awake()
    {
        // Initialize the generated class
        inputActions = new PlayerInput();
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
    }

    // Update is called once per frame
    void Update()
    {
        // Read value from the "Move" action in the "Player" map
        movementInput = inputActions.Player.Move.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Apply velocity in FixedUpdate for consistent physics behavior
        // Note: Using linearVelocity (Unity 6+) or velocity (older versions)
        rb.linearVelocity = movementInput * moveSpeed;
    }
}
