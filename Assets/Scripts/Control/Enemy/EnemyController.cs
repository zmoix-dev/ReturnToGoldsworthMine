using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Game.Animation;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control.Enemy {
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float aggroRadius = 5f;
        [SerializeField] float suspicionTime = 2f;
        [SerializeField] PatrolPath path;
        [SerializeField] float guardDestinationTolerance = 2f;
        [SerializeField] float patrolSpeed = 2.5f;
        [SerializeField] float chaseSpeed = 4f;
        [SerializeField] float waitAtWaypoint = 4f;

        Vector3 guardDestination;
        int guardDestinationIndex;
        Fighter fighter;
        Mover mover;
        GameObject player;
        NavMeshAgent navMeshAgent;
        bool isChasing = false;
        bool isWaiting = false;

        void Start() {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            
            if (path != null) {
                // Start guard at random point on path
                guardDestinationIndex = UnityEngine.Random.Range(0, path.GetWaypointCount());
                guardDestination = path.GetWaypoint(guardDestinationIndex);
                transform.position = guardDestination;
                // Set next patrol destination
                guardDestination = path.GetWaypoint(++guardDestinationIndex);
                GetComponent<NavMeshAgent>().speed = patrolSpeed;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<Health>().IsDead) {
                this.enabled = false;
                fighter.enabled = false;
                mover.enabled = false;
            }
            HandleChase();
        }

        private void HandleChase()
        {
            if(FindPlayerDistance() <= aggroRadius)
            {
                PursueBehavior();
            }
            else if (isChasing) {
                StartCoroutine(ResetBehavior());
            }
            else {
                if (!isWaiting) {
                    StartCoroutine(PatrolBehavior());
                }
                
            }
        }

        private void PursueBehavior()
        {
            navMeshAgent.speed = chaseSpeed;
            fighter.SelectTarget(player);
            isChasing = true;
        }

        private IEnumerator ResetBehavior()
        {
            GetComponent<ActionScheduler>().StopCurrentAction();
            isChasing = false;
            yield return new WaitForSeconds(suspicionTime);
            navMeshAgent.speed = patrolSpeed;
            mover.StartMoveAction(guardDestination);
        }

        private IEnumerator PatrolBehavior()
        {
            if (path != null) {
                if (AtWaypoint()) {
                    isWaiting = true;
                    yield return new WaitForSeconds(waitAtWaypoint);
                    guardDestination = path.GetWaypoint(++guardDestinationIndex);
                    isWaiting = false;
                }

            }
            mover.StartMoveAction(guardDestination);
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, guardDestination) <= guardDestinationTolerance;
        }

        private float FindPlayerDistance()
        {
            if (!player) {
                return float.MaxValue;
            }
            return Vector3.Distance(transform.position, player.transform.position);
        }

        // Called by Unity
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);
        }
    }
}