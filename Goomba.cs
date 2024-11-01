using UnityEngine;

public class Goomba : MonoBehaviour
{
    public int scoreValue = 100;         // Points to add when Goomba is defeated
    public float speed = 2f;             // Speed at which Goomba moves
    private int direction = -1;          // Direction: -1 for left, 1 for right
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Ensure Goomba doesn't fall
    }

    void Update()
    {
        // Move Goomba horizontally based on direction
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if collision is from the top by comparing player's and Goomba's Y positions
            float playerY = collision.gameObject.transform.position.y;
            float goombaY = transform.position.y;

            if (playerY > goombaY + 0.1f) // Offset to ensure it's from above
            {
                // Mario hit the Goomba from the top, so kill the Goomba
                Die();
                // Optionally, add a bounce effect on Mario when he defeats Goomba
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f); // Adjust 10f as needed for bounce
                }
            }
            else
            {
                // Mario collided from the side, so he should take damage or die
                GameManager.instance.PlayerHit(); // Assuming PlayerHit reduces Mario's lives or triggers Game Over
            }
        }
        else if (collision.gameObject.CompareTag("Pipe") || collision.gameObject.CompareTag("Ground"))
        {
            // Reverse direction when hitting a pipe or ground
            ReverseDirection();
        }
    }

    public void Die()  // Renamed Defeat to Die
    {
        // Increase Mario's score using GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(scoreValue);
        }
        else
        {
            Debug.LogError("GameManager instance is missing in the scene.");
        }

        // Destroy Goomba game object to remove it from the scene
        Destroy(gameObject);
    }

    private void ReverseDirection()
    {
        // Reverse the direction and flip the sprite
        direction *= -1;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
