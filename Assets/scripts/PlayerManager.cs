using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class PlayerManager : MonoBehaviour
{
    public List<Player> players;
    public int laps = 5; // Set the number of laps here
    public GameObject endGameCanvas;
    public TextMeshProUGUI endGameText;
    public int totalScore = 0; // Total score managed by PlayerManager

    private void Start()
    {
        foreach (Player player in players)
        {
            player.Initialize(laps);
        }
        endGameCanvas.SetActive(false); // Ensure the end game canvas is initially disabled
        Debug.Log("PlayerManager started");
    }

    private void Update()
    {
        foreach (Player player in players)
        {
            player.UpdateCheckpoint();
            if (player.HasFinished())
            {
                Debug.Log("Player has finished: " + player.playerObject.name);
                EndGame(player);
            }
        }
    }

    private void EndGame(Player winner)
    {
        endGameCanvas.SetActive(true);
        endGameText.text = winner == players[0] ? "You Won!" : "You Lost!";
        StartCoroutine(EndGameCountdown());
        Debug.Log("EndGame triggered for: " + winner.playerObject.name);
    }

    private IEnumerator EndGameCountdown()
    {
        yield return new WaitForSeconds(5);
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        Debug.Log("Main menu loaded");
    }

    public void AddScore(int points)
    {
        totalScore += points;
        Debug.Log("Total Score: " + totalScore);
    }
}


[System.Serializable]
public class Player
{
    public GameObject playerObject;
    public int score = 0;
    public int currentLap = 0;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI lapText;
    public CheckpointManager checkpointManager;
    private int totalLaps;
    private int checkpointsPerLap;

    public void Initialize(int totalLaps)
    {
        this.totalLaps = totalLaps;
        this.checkpointsPerLap = checkpointManager.checkpoints.Count;
        if (positionText != null) positionText.text = "Position: 1";
        if (lapText != null) lapText.text = $"Lap: {currentLap}/{totalLaps}";
        Debug.Log("Player initialized: " + playerObject.name);
    }

    public void UpdateCheckpoint()
    {
        Transform nextCheckpoint = checkpointManager.GetNextCheckpoint();
        if (nextCheckpoint != null && Vector3.Distance(playerObject.transform.position, nextCheckpoint.position) < 1.0f)
        {
            checkpointManager.ReachedCheckpoint();
            score++;
            GameObject.FindObjectOfType<PlayerManager>().AddScore(1); // Add to total score

            if (checkpointManager.GetNextCheckpoint() == checkpointManager.checkpoints[0])
            {
                currentLap++;
                Debug.Log($"Player {playerObject.name} completed lap {currentLap}/{totalLaps} with score {score}");

                if (currentLap >= totalLaps)
                {
                    currentLap = totalLaps;
                }
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (positionText != null) positionText.text = "Position: " + (score + 1);
        if (lapText != null) lapText.text = $"Lap: {currentLap}/{totalLaps}";
    }

    public bool HasFinished()
    {
        return currentLap >= totalLaps && checkpointManager.GetNextCheckpoint() == checkpointManager.checkpoints[0];
    }
}