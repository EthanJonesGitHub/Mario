using UnityEngine;

public class MarioMovement : MonoBehaviour
{
    public float walkSpeed = 6f;         // Normal walking speed for Mario's movement
    public float runMultiplier = 1.3f;   // Lowered multiplier for Mario's speed when running
    public float jumpForce = 10f;        // Force applied when Mario jumps
    public float runJumpMultiplier = 1.1f; // Slight multiplier for jump force when running
    private bool isGrounded;             // Check if Mario is on the ground

    private Rigidbody2D rb;              // Mario's Rigidbody2D component
    private Animator animator;           // Animator for Mario's animations

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();     // Access Rigidbody2D component
        animator = GetComponent<Animator>();  // Access Animator component

        // Enable interpolation on the Rigidbody2D for smoother movement
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // Determine if Mario is running by holding the "Fire3" button (mapped to X button)
        bool isRunning = Input.GetButton("Fire3");
        float currentSpeed = isRunning ? walkSpeed * runMultiplier : walkSpeed;

        // Horizontal movement input (works with keyboard and gamepad joystick)
        float moveInput = Input.GetAxis("Horizontal");

        // Apply horizontal movement only if Mario is grounded; otherwise, maintain existing momentum in the air
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Allow for slight air control while maintaining momentum
            rb.linearVelocity = new Vector2(moveInput * walkSpeed, rb.linearVelocity.y);
        }

        // Flip Mario's sprite based on direction
        if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveInput > 0)
            transform.localScale = new Vector3(1, 1, 1);

        // Jumping logic (works with spacebar and A button on Xbox)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            float jumpStrength = isRunning ? jumpForce * runJumpMultiplier : jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
            isGrounded = false; // Set grounded to false when jumping
        }

        // Update animator parameters
        animator.SetFloat("Speed", Mathf.Abs(moveInput * currentSpeed));
    }

    // Check if Mario is grounded using collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
        {
            isGrounded = true;  // Mario is grounded when colliding with ground objects
        }
    }
}
