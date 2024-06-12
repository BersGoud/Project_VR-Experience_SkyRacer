using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames.Airplane
{
    public class SimpleAirPlaneColliderAgent : MonoBehaviour
    {
        public bool collideSometing;

        [HideInInspector]
        public SimpleAirPlaneControllerAgent controller;

        private void OnTriggerEnter(Collider other)
        {
            //Collide someting bad
            if(other.gameObject.GetComponent<SimpleAirPlaneColliderAgent>() == null && other.gameObject.GetComponent<LandingArea>() == null)
            {
                collideSometing = true;
            }
        }
    }
}