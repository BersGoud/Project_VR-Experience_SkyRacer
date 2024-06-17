using HeneGames.Airplane;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI heightText;
    [SerializeField] private TextMeshProUGUI crashText;  // Add this line
    [SerializeField] private SimpleAirPlaneController airplaneController;
    [SerializeField] private Transform airplaneTransform;
    [SerializeField] private AudioSource lowHeightAudioSource;

    private void Start()
    {
        if (crashText != null)
        {
            crashText.text = "";
        }
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateHeight();
    }

    private void UpdateSpeed()
    {
        if (airplaneController != null && speedText != null)
        {
            float speed = airplaneController.IsTurboActive ? airplaneController.GetTurboSpeed() : airplaneController.GetDefaultSpeed();
            int speedInt = Mathf.RoundToInt(speed);
            speedText.text = $"SPD\n{speedInt}";
        }
    }

    private void UpdateHeight()
    {
        if (airplaneTransform != null && heightText != null)
        {
            float height = airplaneTransform.position.y - GetTerrainHeightAtPosition(airplaneTransform.position);
            int heightInt = Mathf.RoundToInt(height);
            heightText.text = $"ELV\n{heightInt}";

            // Play or stop audio based on height
            if (height < 10f)
            {
                if (!lowHeightAudioSource.isPlaying)
                {
                    lowHeightAudioSource.Play();
                }
            }
            else
            {
                if (lowHeightAudioSource.isPlaying)
                {
                    lowHeightAudioSource.Stop();
                }
            }
        }
    }

    private float GetTerrainHeightAtPosition(Vector3 position)
    {
        if (Terrain.activeTerrain != null)
        {
            return Terrain.activeTerrain.SampleHeight(position);
        }
        return 0f;
    }

    public void ShowCrashMessage()  // Add this method
    {
        if (crashText != null)
        {
            crashText.text = "[CRASH]\nRESTART GAME";
        }
    }
}
