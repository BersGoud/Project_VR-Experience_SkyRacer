using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private XRInteractorLineVisual rightInteractorLineVisual;
    [SerializeField] private XRRayInteractor rightRayInteractor; // Reference to the right controller's ray interactor
    [SerializeField] private InputActionReference pauseAction; // Reference to the action for the Y button
    [SerializeField] private InputActionReference triggerAction; // Reference to the action for the trigger button
    [SerializeField] private AudioSource gameAudioSource; // Reference to the game audio source
    [SerializeField] private MonoBehaviour[] scriptsToDisable; // References to scripts/components to disable during pause

    private bool isPaused = false;

    private void OnEnable()
    {
        pauseAction.action.performed += OnPause;
        triggerAction.action.performed += OnTriggerClick;
        pauseAction.action.Enable();
        triggerAction.action.Enable();
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        triggerAction.action.performed -= OnTriggerClick;
        pauseAction.action.Disable();
        triggerAction.action.Disable();
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void OnTriggerClick(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            // Handle button clicks if needed
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze the game time
        pausePanel.SetActive(true);
        EnableInteractor(true);
        gameAudioSource.Pause(); // Pause the game audio

        // Disable other game scripts/components to freeze the game logic
        foreach (var script in scriptsToDisable)
        {
            script.enabled = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Unfreeze the game time
        pausePanel.SetActive(false);
        EnableInteractor(false);
        gameAudioSource.UnPause(); // Resume the game audio

        // Enable other game scripts/components to resume the game logic
        foreach (var script in scriptsToDisable)
        {
            script.enabled = true;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset
        SceneManager.LoadScene("MainMenu"); // Load main menu scene, make sure to replace with your actual main menu scene name
    }

    private void EnableInteractor(bool enable)
    {
        rightInteractorLineVisual.enabled = enable;
        rightRayInteractor.enabled = enable;

        // If there's a Line Renderer component, ensure it is enabled/disabled
        LineRenderer lineRenderer = rightInteractorLineVisual.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = enable;
        }
    }
}
