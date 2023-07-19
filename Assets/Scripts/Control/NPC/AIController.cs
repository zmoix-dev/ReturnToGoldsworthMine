using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control {
    public class AIController : MonoBehaviour
    {
        [SerializeField] float aggroRadius = 5f;
        [SerializeField] float suspicionTime = 2f;
        [SerializeField] float aggravateTimer = 5f;
        [SerializeField] PatrolPath path;
        [SerializeField] float guardDestinationTolerance = 2f;
        [SerializeField] float patrolSpeed = 2.5f;
        [SerializeField] float chaseSpeed = 4f;
        [SerializeField] float waitAtDestination = 4f;
        [SerializeField] float wanderRadius = 0f;
        [SerializeField] float alertRadius = 5f;
        [SerializeField] UnitType.Type[] enemyFaction;

        Vector3 guardDestination;
        Vector3 wanderGuardDestination;
        int guardDestinationIndex;
        Fighter fighter;
        Mover mover;
        List<GameObject> enemies;
        GameObject target;
        NavMeshAgent navMeshAgent;
        bool isChasing = false;
        bool isWaiting = false;
        bool isAggravated = false;
        Coroutine aggravateHandler;

        private void Awake() {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            enemies = new List<GameObject>();
            
            if (path != null) {
                // Start guard at random point on path
                guardDestinationIndex = UnityEngine.Random.Range(0, path.GetWaypointCount());
                guardDestination = path.GetWaypoint(guardDestinationIndex);
                wanderGuardDestination = guardDestination;
                transform.position = guardDestination;
                // Set next patrol destination
                guardDestination = path.GetWaypoint(++guardDestinationIndex);
                GetComponent<NavMeshAgent>().speed = patrolSpeed;
            } else {
                Debug.Log($"No path set for {name}.");
            }
        }

        private void Update()
        {
            if (GetComponent<Health>().IsDead)
            {
                this.enabled = false;
                fighter.enabled = false;
                mover.enabled = false;
                return;
            }
            if (enemies.Count == 0) {
                SearchForEnemies();
            }
            HandleChase();
        }

        private void SearchForEnemies()
        {
            foreach (UnitType.Type type in enemyFaction)
            {
                enemies.AddRange(GameObject.FindGameObjectsWithTag(UnitType.GetType(type)));
            }
        }

        private void HandleChase()
        {
            if (isChasing) return;
            if (isAggravated) {
                StartCoroutine(PursueBehavior(target));
                return;
            }

            Debug.Log($"{name} thinking...");
            foreach (GameObject enemy in enemies) {
                
                if(CanAttack(enemy))
                {
                    StopAllCoroutines();
                    StartCoroutine(PursueBehavior(enemy));
                    return;
                }
            }

            if (isChasing) {
                StartCoroutine(ResetBehavior());
            }
            else {
                StartCoroutine(PatrolBehavior());
            }
        }

        public void AggravateController(float damage, GameObject aggravator) {
            if (aggravateHandler != null) {
                StopCoroutine(aggravateHandler);
            }
            AggravateNearby(aggravator);
        }

        private void AggravateNearby(GameObject target)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, alertRadius, Vector3.up, 0);
            foreach (RaycastHit hit in hits) {
                // enemies alert enemies, npcs alert npcs
                if (hit.collider.transform.tag.Equals(gameObject.tag)) {
                    AIController enemy = hit.collider.transform.gameObject.GetComponent<AIController>();
                    if (enemy != null) {
                        StartCoroutine(enemy.Aggravate(target));
                    }
                }
            }
        }

        private IEnumerator Aggravate(GameObject target) {
            SearchForEnemies();
            // if already aggravated, ignore
            if (isChasing) yield return null;
            else {
                this.target = target;
                isAggravated = true;
                yield return new WaitForSeconds(aggravateTimer);
                isAggravated = false;
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
            Debug.Log($"{name} entering PursuitBehavior");
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
            Debug.Log($"{name} entering ResetBehavior");
            GetComponent<ActionScheduler>().StopCurrentAction();
            isChasing = false;
            yield return new WaitForSeconds(suspicionTime);
            navMeshAgent.speed = patrolSpeed;
            mover.StartMoveAction(guardDestination);
        }

        private IEnumerator PatrolBehavior()
        {
            Debug.Log($"{name} entering PatrolBehavior");
            navMeshAgent.speed = patrolSpeed;
            if (path != null) {
                if (AtWaypoint(wanderGuardDestination)) {
                    isWaiting = true;
                    SearchForEnemies();
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
            Gizmos.DrawWireSphere(transform.position, alertRadius);
        }
    }
}