using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction, IJsonSaveable
    {
        [SerializeField] float maxNavPathLength = 40f;
        NavMeshAgent navAgent;
        
        void Awake() {
            navAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if (GetComponent<Health>().IsDead) return;
            UpdateAnimator();
        }

        public void Stop() {
            navAgent.isStopped = true;
        }

        public void StartMoveAction(Vector3 destination) {
            MoveTo(destination);
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public Vector3 StartMoveAction(Vector3 destination, float wanderRadius) {
            float xWander = UnityEngine.Random.Range(-wanderRadius, wanderRadius);
            float zWander = UnityEngine.Random.Range(-wanderRadius, wanderRadius);
            destination.x += xWander;
            destination.z += zWander;
            StartMoveAction(destination);
            return destination;
        }

        public bool CanMoveTo(Vector3 destination) {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) {
                return false;
            }
            // if (path.status == NavMeshPathStatus.PathComplete) {
            //     Debug.Log("Incomplete path");
            //     return false;
            // }
            if (GetPathLength(path) > maxNavPathLength) {
                return false;
            }
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++) {
                totalDistance += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }
            return totalDistance;
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

        public JToken CaptureAsJToken()
        {
           return transform.position.ToToken();
        }

        public void RestoreFromJToken(JToken state)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = state.ToVector3();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().StopCurrentAction();
        }
    }
}