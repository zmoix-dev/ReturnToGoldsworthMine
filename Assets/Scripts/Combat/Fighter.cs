using System;
using System.Collections;
using RPG.Core;
using RPG.Game.Animation;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] Transform handTransform;
        [SerializeField] Weapon defaultWeapon;
        Weapon equippedWeapon;
       
        GameObject target = null;
        bool canAttack = true;
        Mover mover;

        void Start() {
            mover = GetComponent<Mover>();
            EquipWeapon(defaultWeapon);
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
            if (target != null) {
                target.GetComponent<Health>().TakeDamage(equippedWeapon.WeaponDamage);
                if (target.GetComponent<Health>().IsDead) {
                    Stop();
                }
            }
        }

        private void ChaseTarget()
        {
            if (target != null) {
                if (Vector3.Distance(transform.position, target.transform.position) <= equippedWeapon.AttackRange)
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
            yield return new WaitForSeconds(equippedWeapon.TimeBetweenAttacks);
            canAttack = true;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon) {
                equippedWeapon = weapon;
                weapon.Spawn(handTransform, GetComponent<Animator>());
            } else {
                Debug.LogError($"No weapon equipped to Fighter on {name}.");
            }
        }

        public void EquipWeapon(WeaponPickup pickup)
        {
            if (pickup) {
                Weapon weapon = pickup.Weapon;
                equippedWeapon = weapon;
                weapon.Spawn(handTransform, GetComponent<Animator>());
                Destroy(pickup.gameObject);
            } else {
                Debug.LogError($"No weapon equipped to Fighter on {name}.");
            }
        }
    }
}