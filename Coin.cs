using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinValue = 100; // Points each coin is worth

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is specifically Mario (Player)
        if (other.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(coinValue); // Increase score
                GameManager.instance.AddCoin();
            }
            Destroy(gameObject); // Remove the coin from the game
        }
    }
}
