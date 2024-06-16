using HeneGames.Airplane;
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
    [SerializeField] private XRRayInteractor rightRayInteractor;
    [SerializeField] private XRInteractorLineVisual leftInteractorLineVisual; // Add this for left hand
    [SerializeField] private XRRayInteractor leftRayInteractor; // Add this for left hand
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private InputActionReference triggerAction;
    [SerializeField] private AudioSource gameAudioSource;
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private bool isPaused = false;
    private SimpleAirPlaneController planeController;

    private void Start()
    {
        planeController = FindObjectOfType<SimpleAirPlaneController>();
        if (planeController == null)
        {
            Debug.LogError("SimpleAirPlaneController not found in the scene.");
        }
    }

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
            float triggerValue = context.ReadValue<float>();
            if (triggerValue > 0.5f)
            {
                PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
                pointerEventData.position = new Vector2(Screen.width / 2, Screen.height / 2); // Assuming center of the screen for simplicity
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerEventData, results);
                if (results.Count > 0)
                {
                    // Handle the UI click
                    ExecuteEvents.Execute(results[0].gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                }
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        EnableInteractor(true);
        gameAudioSource.Pause();

        foreach (var script in scriptsToDisable)
        {
            script.enabled = false;
        }

        if (planeController != null)
        {
            planeController.vrJoystickMode = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        EnableInteractor(false);
        gameAudioSource.UnPause();

        foreach (var script in scriptsToDisable)
        {
            script.enabled = true;
        }

        if (planeController != null)
        {
            planeController.vrJoystickMode = true;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void EnableInteractor(bool enable)
    {
        rightInteractorLineVisual.enabled = enable;
        rightRayInteractor.enabled = enable;
        leftInteractorLineVisual.enabled = enable; // Enable left hand interaction
        leftRayInteractor.enabled = enable; // Enable left hand interaction

        LineRenderer rightLineRenderer = rightInteractorLineVisual.GetComponent<LineRenderer>();
        if (rightLineRenderer != null)
        {
            rightLineRenderer.enabled = enable;
        }

        LineRenderer leftLineRenderer = leftInteractorLineVisual.GetComponent<LineRenderer>();
        if (leftLineRenderer != null)
        {
            leftLineRenderer.enabled = enable;
        }
    }
}
