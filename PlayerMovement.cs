using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 6f;
    public float runMultiplier = 1.3f;
    public float jumpForce = 10f;
    public float runJumpMultiplier = 1.1f;
    public GameObject fireballPrefab;
    private bool hasFirePower = false;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Ensure Mario is facing right by default
        transform.localScale = new Vector3(1, 1, 1);

        // Set initial speed to 0 to ensure idle animation plays
        animator.SetFloat("Speed", 0);
    }

    void Update()
    {
        bool isRunning = Input.GetButton("Fire3");
        float currentSpeed = isRunning ? walkSpeed * runMultiplier : walkSpeed;

        float moveInput = Input.GetAxis("Horizontal");

        // Apply horizontal movement
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // Flip Mario's sprite based on movement direction
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }

        // Jumping logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            float jumpStrength = isRunning ? jumpForce * runJumpMultiplier : jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
            isGrounded = false;
        }

        // Update the Animator's Speed parameter based on movement
        animator.SetFloat("Speed", Mathf.Abs(moveInput * currentSpeed));

        // Fireball shooting logic with "Fire3" button
        if (hasFirePower && Input.GetButtonDown("Fire3"))
        {
            ShootFireball();
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab == null)
        {
            Debug.LogError("Fireball Prefab is not assigned in the Inspector!");
            return;
        }

        Vector2 fireballPosition = new Vector2(transform.position.x + (transform.localScale.x * 0.5f), transform.position.y);
        GameObject fireball = Instantiate(fireballPrefab, fireballPosition, Quaternion.identity);

        fireball.GetComponent<Fireball>().InitializeFireball(transform.localScale.x);
    }

    public void ActivateFirePower()
    {
        hasFirePower = true;
        animator.SetBool("hasFirePower", true);
    }

    public void LoseFirePower()
    {
        hasFirePower = false;
        animator.SetBool("hasFirePower", false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FireFlower"))
        {
            ActivateFirePower();
            Destroy(collision.gameObject);
        }
    }
}
