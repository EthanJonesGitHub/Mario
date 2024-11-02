using UnityEngine;
using System.Collections;

public class KoopaTroopa : MonoBehaviour
{
    public int scoreValue = 200;             // Points when defeated
    public float speed = 1f;                 // Normal movement speed for Koopa Troopa
    public float slideSpeed = 5f;            // Speed of the shell when sliding
    public float flyHeight = 1.5f;           // Height for flying up and down
    public bool hasWings = true;             // Tracks if Koopa Troopa has wings
    public Sprite winglessSprite;            // Sprite for wingless state
    public Sprite shellSprite;               // Sprite for shell form

    private Rigidbody2D rb;
    private Vector3 initialPosition;         // Initial position for flying range
    private bool isShell = false;            // Tracks if Koopa Troopa is in shell form
    private bool isSliding = false;          // Tracks if the shell is sliding
    private bool isHeld = false;             // Tracks if shell is held by Mario
    private int direction = -1;              // Movement direction
    private int slideDirection = 1;          // Sliding direction
    private Coroutine revertCoroutine;       // Coroutine for reverting back to Koopa Troopa

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        initialPosition = transform.position;
    }

    void Update()
    {
        if (isShell && isSliding && !isHeld)
        {
            // Shell slides continuously when in sliding mode
            rb.linearVelocity = new Vector2(slideDirection * slideSpeed, rb.linearVelocity.y);
        }
        else if (!isShell && !isHeld)
        {
            if (hasWings)
            {
                // Winged movement (flying up and down)
                float newY = initialPosition.y + Mathf.PingPong(Time.time * speed, flyHeight);
                transform.position = new Vector3(transform.position.x + (direction * speed * Time.deltaTime), newY, transform.position.z);
            }
            else
            {
                // Normal Koopa Troopa movement
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            }
        }

        // Ensure gravity is applied when the shell is in the air and not held or sliding
        if (isShell && !isHeld && !isSliding)
        {
            rb.gravityScale = 1;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isShell && isSliding && !isHeld)
        {
            // Handle shell collision with Mario while sliding
            if (collision.gameObject.CompareTag("Player"))
            {
                // Stop the shell instead of killing Mario immediately
                StopSliding();
            }
            else if (collision.gameObject.CompareTag("Goomba"))
            {
                // Defeat the Goomba and add score
                Goomba goomba = collision.gameObject.GetComponent<Goomba>();
                if (goomba != null)
                {
                    goomba.Die(); // Call Goomba's Die method to handle score and destruction
                    GameManager.instance.AddScore(scoreValue); // Add 200 points for defeating Goomba
                }
            }
            else if (collision.gameObject.CompareTag("Enemy"))
            {
                Destroy(collision.gameObject); // Destroy other generic enemies if necessary
            }
            else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
            {
                slideDirection *= -1; // Reverse direction upon hitting walls or obstacles
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            float playerY = collision.gameObject.transform.position.y;
            float koopaY = transform.position.y;

            if (!isShell)
            {
                // First stomp, turn into a shell
                if (playerY > koopaY + 0.1f)  // Stomp from above
                {
                    BecomeShell();
                }
                else
                {
                    GameManager.instance.PlayerHit(); // Hurt Mario on side collision
                }
            }
            else if (!isSliding)
            {
                // Mario stomps on the shell again to make it slide
                if (playerY > koopaY + 0.1f)
                {
                    StartSliding();
                }
            }
            else
            {
                // Mario stomps on the sliding shell to stop it
                if (playerY > koopaY + 0.1f)
                {
                    StopSliding();
                }
            }
        }
        else if (collision.gameObject.CompareTag("Pipe") || collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
        {
            ReverseDirection();
        }
    }

    private void BecomeShell()
    {
        isShell = true;
        isSliding = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 1; // Enable gravity so the shell falls if it's in the air
        GetComponent<SpriteRenderer>().sprite = shellSprite;
        gameObject.tag = "Shell"; // Change tag to "Shell" for detection

        // Start the revert coroutine if it's not already running
        if (revertCoroutine == null)
        {
            revertCoroutine = StartCoroutine(RevertToKoopaTroopa());
        }
    }

    private void StartSliding()
    {
        isSliding = true;
        slideDirection = direction; // Set sliding direction based on current direction

        // Set initial sliding velocity
        rb.linearVelocity = new Vector2(slideDirection * slideSpeed, rb.linearVelocity.y);

        // Stop the revert coroutine as it's now sliding again
        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
            revertCoroutine = null;
        }
    }

    private void StopSliding()
    {
        isSliding = false;
        rb.linearVelocity = Vector2.zero; // Stop the shell's movement

        // Restart the revert coroutine to turn back into a Koopa Troopa after 5 seconds
        if (revertCoroutine == null)
        {
            revertCoroutine = StartCoroutine(RevertToKoopaTroopa());
        }
    }

    private IEnumerator RevertToKoopaTroopa()
    {
        yield return new WaitForSeconds(5f); // Wait 5 seconds

        // Revert back to normal Koopa Troopa
        isShell = false;
        isSliding = false;
        rb.gravityScale = 1;
        GetComponent<SpriteRenderer>().sprite = winglessSprite;
        gameObject.tag = "Enemy";  // Change tag back to "Enemy"
        direction = -1;            // Reset direction if needed
        hasWings = false;          // Koopa Troopa will be wingless when it reverts
        revertCoroutine = null;
    }

    public void PickUp(Transform holdPoint)
    {
        isHeld = true;
        isSliding = false;
        rb.bodyType = RigidbodyType2D.Kinematic; // Set to Kinematic while being held
        rb.linearVelocity = Vector2.zero;
        transform.position = holdPoint.position; // Attach to the hold point
        transform.parent = holdPoint; // Parent the shell to the hold point so it moves with Mario
    }

    public void Release(Vector2 throwDirection)
    {
        isHeld = false;
        isSliding = true;
        rb.bodyType = RigidbodyType2D.Dynamic; // Set to Dynamic after release
        rb.gravityScale = 1; // Re-enable gravity after release
        transform.parent = null; // Unparent from Mario
        rb.linearVelocity = throwDirection.normalized * slideSpeed; // Set velocity on release in the throw direction
    }

    private void ReverseDirection()
    {
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
