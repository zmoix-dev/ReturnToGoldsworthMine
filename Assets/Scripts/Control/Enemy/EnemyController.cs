using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
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
        [SerializeField] float waitAtDestination = 4f;
        [SerializeField] float wanderRadius = 0f;
        [SerializeField] UnitType.UnitTypes[] enemyFaction;

        Vector3 guardDestination;
        Vector3 wanderGuardDestination;
        int guardDestinationIndex;
        Fighter fighter;
        Mover mover;
        List<GameObject> enemies;
        NavMeshAgent navMeshAgent;
        bool isChasing = false;
        bool isWaiting = false;

        void Start() {
            enemies = new List<GameObject>();
            foreach (UnitType.UnitTypes type in enemyFaction) {
                enemies.AddRange(GameObject.FindGameObjectsWithTag(UnitType.GetUnitType(type)));    
            }
            
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            
            if (path != null) {
                // Start guard at random point on path
                guardDestinationIndex = UnityEngine.Random.Range(0, path.GetWaypointCount());
                guardDestination = path.GetWaypoint(guardDestinationIndex);
                wanderGuardDestination = guardDestination;
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
            if (isChasing) return;
            foreach (GameObject enemy in enemies) {
                if(CanAttack(enemy))
                {
                    StopAllCoroutines();
                    StartCoroutine(PursueBehavior(enemy));
                    break;
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
        }

        private bool CanAttack(GameObject enemy)
        {
            return FindTargetDistance(enemy) <= aggroRadius &&
                enemy.GetComponent<Health>() != null &&
                !enemy.GetComponent<Health>().IsDead;
        }

        private IEnumerator PursueBehavior(GameObject enemy)
        {
            navMeshAgent.speed = chaseSpeed;
            fighter.SelectTarget(enemy);
            isChasing = true;
            while (isChasing) {
                isChasing = CanAttack(enemy);
                yield return new WaitForEndOfFrame();
            }
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
            navMeshAgent.speed = patrolSpeed;
            if (path != null) {
                if (AtWaypoint(wanderGuardDestination)) {
                    isWaiting = true;
                    yield return new WaitForSeconds(waitAtDestination);
                    isWaiting = false;  
                    wanderGuardDestination = mover.StartMoveAction(guardDestination, wanderRadius);
                }
            }
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, guardDestination) <= guardDestinationTolerance;
        }

        private bool AtWaypoint(Vector3 waypoint, bool ignoreY = true)
        {
            if (ignoreY) {
                Vector2 newPosition = new Vector2(transform.position.x, transform.position.z);
                Vector2 newWaypoint = new Vector2(waypoint.x, waypoint.z);
                return Vector2.Distance(newPosition, newWaypoint) <= guardDestinationTolerance;
            } else {
                return Vector3.Distance(transform.position, waypoint) <= guardDestinationTolerance;
            }
        }

        private float FindTargetDistance(GameObject target)
        {
            if (!target) {
                return float.MaxValue;
            }
            return Vector3.Distance(transform.position, target.transform.position);
        }

        // Called by Unity
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(path.GetWaypoint(0), wanderRadius);
        }
    }
}