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
            // Collide with something bad
            if (other.gameObject.tag == "Checkpoint") {
                if (controller.isAgent)
                {
                    agent.HandleCheckpointCollision(other);
                }
            }
            else if (other.gameObject.GetComponent<SimpleAirPlaneCollider>() == null &&
                     other.gameObject.GetComponent<LandingArea>() == null)
            {
                collideSometing = true;
            }
        }
    }
}

