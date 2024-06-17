using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HeneGames.Airplane
{
    public class SimpleAirPlaneCollider : MonoBehaviour
    {
        public bool collideSometing;

        [HideInInspector]
        public SimpleAirPlaneController controller;

        private AirplaneAgent agent;


        private void Start()
        {
            if (controller.isAgent)
            {
                agent = GetComponentInParent<AirplaneAgent>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {

            if (other.gameObject.tag == "Checkpoint")
            {
                if (controller.isAgent)
                {
                    agent.HandleCheckpointCollision(other);
                }
                else
                {
                    GameManager.instance.AddScore(1);
                    AudioSource checkpointAudioSource = other.GetComponent<AudioSource>();
                    if (checkpointAudioSource != null)
                    {
                        // Play the audio clip
                        checkpointAudioSource.Play();
                    }
                }
            }
            // Collide with something bad
            else if (other.gameObject.GetComponent<SimpleAirPlaneCollider>() == null &&
                     other.gameObject.GetComponent<LandingArea>() == null)
            {
                collideSometing = true;
            }
        }
    }
}

