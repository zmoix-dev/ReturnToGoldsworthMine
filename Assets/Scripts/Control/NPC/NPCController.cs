using System.Collections;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control.Enemy {
    public class NPCController : MonoBehaviour
    {
        [SerializeField] float aggroRadius = 5f;
        [SerializeField] float suspicionTime = 2f;
        [SerializeField] PatrolPath path;
        [SerializeField] float guardDestinationTolerance = 2f;
        [SerializeField] float patrolSpeed = 2.5f;
        [SerializeField] float chaseSpeed = 4f;
        [SerializeField] float waitAtWaypoint = 4f;
        [SerializeField] UnitType.Type enemyFaction;

        Vector3 guardDestination;
        int guardDestinationIndex;
        Fighter fighter;
        Mover mover;
        GameObject[] enemies;
        NavMeshAgent navMeshAgent;
        bool isChasing = false;
        bool isWaiting = false;

        void Start() {
            enemies = GameObject.FindGameObjectsWithTag(UnitType.GetType(enemyFaction));
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
                fighter.enabled = false;
                mover.enabled = false;
                this.enabled = false;
            }
            HandleChase();
        }

        private void HandleChase()
        {
            // if engaged with something, continue engaging with that something
            if (isChasing) { return; }

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

        private bool CanAttack(GameObject target)
        {
            return FindEnemyDistance(target) <= aggroRadius && IsTargetAlive(target);    
        }

        private bool IsTargetAlive(GameObject target) {
            return target.GetComponent<Health>() != null &&
                !target.GetComponent<Health>().IsDead;
        }

        private IEnumerator PursueBehavior(GameObject enemy)
        {
            navMeshAgent.speed = chaseSpeed;
            fighter.SelectTarget(enemy);
            isChasing = true;
            while (isChasing) {
                yield return new WaitForEndOfFrame();
                isChasing = CanAttack(enemy);
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

        private float FindEnemyDistance(GameObject enemy)
        {
            if (!enemy) {
                return float.MaxValue;
            }
            return Vector3.Distance(transform.position, enemy.transform.position);
        }

        // Called by Unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, aggroRadius);
        }
    }
}