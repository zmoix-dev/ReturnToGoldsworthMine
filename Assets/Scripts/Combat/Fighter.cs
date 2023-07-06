using System.Collections;
using RPG.Core;
using RPG.Game.Animation;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float attackRange = 3.5f;
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] float weaponDamage = 5f;
        GameObject target;
        bool canAttack = true;
        Mover mover;

        void Start() {
            mover = GetComponent<Mover>();
        }

        void Update() {
            ChaseTarget();
        }

        public void Stop() {
            this.target = null;
            GetComponent<Animator>().ResetTrigger(AnimationStates.ATTACK);
            GetComponent<Animator>().SetTrigger(AnimationStates.STOP_ATTACK);
            StopAllCoroutines();
            canAttack = true;
        }

        public void SelectTarget(GameObject target) {
            this.target = target;
             GetComponent<ActionScheduler>().StartAction(this);
        }

        // Animation Event
        void OnAnimFrameHit() {
            if (target) {
                target.GetComponent<Health>().TakeDamage(weaponDamage);
                if (target.GetComponent<Health>().IsDead) {
                    Stop();
                }
            }
        }

        private void ChaseTarget()
        {
            if (target) {
                if (Vector3.Distance(transform.position, target.transform.position) <= attackRange)
                {
                    mover.Stop();
                    if (canAttack) {
                        StartCoroutine(AttackBehavior());
                    }
                }
                else {
                    mover.MoveTo(target.transform.position);
                    GetComponent<Animator>().SetTrigger(AnimationStates.STOP_ATTACK);
                }
            }
        }

        private IEnumerator AttackBehavior()
        {
            transform.LookAt(target.transform);
            GetComponent<Animator>().ResetTrigger(AnimationStates.STOP_ATTACK);
            GetComponent<Animator>().SetTrigger(AnimationStates.ATTACK);
            canAttack = false;
            yield return new WaitForSeconds(timeBetweenAttacks);
            canAttack = true;
        }
    }
}