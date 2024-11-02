using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int startingLives = 3;  // Starting lives for Mario
    public int startingScore = 0;  // Starting score for Mario
    public int score = 0;
    public int lives = 3;
    private int coins = 0;
    private const int coinsPerLife = 100;

    public TMP_Text scoreText;
    public TMP_Text livesText;
    public GameObject gameOverScreen;  // Reference to Game Over screen UI
    public GameObject blackOverlay;    // Reference to the black overlay panel
    private bool isGameOver = false;   // Track if the game is over
    private PlayerMovement playerMovement; // Reference to Mario's movement script

    public float fallThreshold = -10f; // Threshold Y position to detect falling
    private Vector3 startingPosition;   // Mario's starting position

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        score = startingScore;    // Initialize score
        lives = startingLives;    // Initialize lives
        UpdateUI();
        gameOverScreen.SetActive(false);
        blackOverlay.SetActive(false);
        playerMovement = GameObject.FindWithTag("Player")?.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            startingPosition = playerMovement.transform.position; // Store Mario's starting position
        }
    }

    void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (!isGameOver && playerMovement != null && playerMovement.transform.position.y < fallThreshold)
        {
            PlayerHit(); // Trigger life loss if Mario falls too far
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    public void AddCoin()
    {
        coins++;
        CheckForExtraLife(); // Check if coins reached 100 to grant extra life
    }

    private void CheckForExtraLife()
    {
        if (coins >= coinsPerLife)
        {
            coins -= coinsPerLife;
            lives++;
            UpdateUI();
        }
    }

    public void PlayerHit()
    {
        if (lives > 1) // Mario still has lives left
        {
            lives--;
            UpdateUI();
            StartCoroutine(ResetMarioPosition()); // Reset Mario’s position only
        }
        else // Game Over scenario when no lives are left
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        gameOverScreen.SetActive(true); // Show Game Over screen
        isGameOver = true; // Set game over flag
        score = startingScore; // Reset score on game over
        UpdateUI();

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        Time.timeScale = 0; // Freeze the game
    }

    public void RestartGame()
    {
        score = startingScore;
        lives = startingLives;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Restart the scene
        gameOverScreen.SetActive(false);
        isGameOver = false;
        Time.timeScale = 1;

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            playerMovement.transform.position = startingPosition;
        }
    }

    private void UpdateUI()
    {
        scoreText.text = "MARIO\n" + score.ToString("000000");
        livesText.text = "x" + lives;
    }

    private IEnumerator ResetMarioPosition()
    {
        blackOverlay.SetActive(true);

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2);

        Time.timeScale = 1;
        blackOverlay.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.transform.position = startingPosition;
        }
    }
}
