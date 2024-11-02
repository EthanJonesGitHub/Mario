using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public float bounceForce = 5f; // Force applied to make the fireball bounce
    public int scoreValue = 200;
    private int direction;

    private Rigidbody2D rb;

    public void InitializeFireball(float marioDirection)
    {
        direction = marioDirection > 0 ? 1 : -1;
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        Destroy(gameObject, 1f);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1; // Enable gravity so fireball falls and bounces
    }

    void Update()
    {
        // Maintain horizontal velocity while allowing vertical velocity for bouncing
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pipe"))
        {
            // Apply bounce force when colliding with ground or pipe
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, bounceForce);
        }
        else if (collision.gameObject.CompareTag("Goomba") || collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the enemy and the fireball
            Destroy(collision.gameObject);
            Destroy(gameObject); // Only this fireball instance is destroyed

            // Award points to Mario (assuming GameManager has a singleton instance)
            GameManager.instance?.AddScore(scoreValue);
        }
    }

    // Destroy the fireball when it goes off-screen
    void OnBecameInvisible()
    {
        Destroy(gameObject); // Only this fireball instance is destroyed
    }
}
