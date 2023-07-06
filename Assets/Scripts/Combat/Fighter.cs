using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Game.Animation;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float attackRange = 3.5f;
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] float weaponDamage = 5f;
        CombatTarget target;
        bool canAttack = true;

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

        public void SelectTarget(CombatTarget target) {
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
                    GetComponent<Mover>().Stop();
                    if (canAttack) {
                        StartCoroutine(AttackBehavior());
                    }
                }
                else {
                    GetComponent<Mover>().MoveTo(target.transform.position);
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