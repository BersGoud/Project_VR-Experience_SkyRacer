using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int score = 0;

    [Header("UI Components")]
    public GameObject positionUI;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI lapText;
    public GameObject endGamePanel;  // Reference to the panel that should be enabled
    public TextMeshProUGUI endGameMessageText;

    private int currentLap = 1;
    private int totalLaps = 3; // Set the default number of laps

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

    public void Start()
    {
        UpdatePositionUI(); // Initialize UI with starting values
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score: " + score);

        // Check if we need to update the lap
        if (score >= 8 && currentLap == 1)
        {
            currentLap = 2;
        }
        else if (score >= 15 && currentLap == 2)
        {
            currentLap = 3;
        }
        else if (score >= 22 && currentLap == 3)
        {
            EndGame();
        }

        UpdatePositionUI();
    }

    public void UpdatePositionUI()
    {
        // Assuming you have a method to get the AI score
        int aiScore = GetAIScore();

        positionText.text = "Position: " + (score > aiScore ? "1st" : "2nd");
        lapText.text = "Lap: " + Mathf.Min(currentLap, totalLaps) + "/" + totalLaps;
    }

    private int GetAIScore()
    {
        // Implement logic to get the AI's score
        return 0; // Placeholder return value
    }

    public void EndGame()
    {
        Time.timeScale = 0.1f; // Slow down the game

        if (score >= 22) // Assuming 22 points to end the game
        {
            endGameMessageText.text = "You Won";
        }
        else
        {
            endGameMessageText.text = "You Lose";
        }

        endGamePanel.SetActive(true);  // Enable the panel

        // Load the main menu after a delay
        Invoke("LoadMainMenu", 0.5f); // 0.5 seconds delay
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale
        // Load the main menu scene (assuming you have a scene named "MainMenu")
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
