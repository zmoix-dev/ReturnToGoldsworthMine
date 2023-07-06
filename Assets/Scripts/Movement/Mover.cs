using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent navAgent;
        
        void Start() {
            navAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        public void Stop() {
            navAgent.isStopped = true;
        }

        public void StartMoveAction(Vector3 destination) {
            MoveTo(destination);
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void MoveTo(Vector3 destination) {
            navAgent.isStopped = false;
            GetComponent<NavMeshAgent>().destination = destination;
        }


        private void UpdateAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }
    }
}