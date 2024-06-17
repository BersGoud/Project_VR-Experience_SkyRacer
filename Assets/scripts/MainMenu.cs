using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Method to quit the game
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Method to start the game
    public void Play()
    {
        Debug.Log("Play Game");
        SceneManager.LoadScene("Game");  // Load the scene named "Game"
    }
}
