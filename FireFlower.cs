using UnityEngine;

public class FireFlower : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement player = collision.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.ActivateFirePower(); // Give Mario fire power
            }
            Destroy(gameObject); // Destroy the Fire Flower after pickup
        }
    }
}
