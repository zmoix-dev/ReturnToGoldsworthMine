using System;
using System.Collections;
using RPG.Core;
using RPG.Game.Animation;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform handTransform;
        [SerializeField] Weapon defaultWeapon = null;
        Weapon equippedWeapon;
        GameObject equippedWeaponObject;
       
        GameObject target = null;
        bool canAttack = true;
        Mover mover;

        void Start() {
            mover = GetComponent<Mover>();
            if (equippedWeapon == null) {
                EquipWeapon(defaultWeapon);
            }
            
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

        public GameObject GetTarget() {
            return target;
        }

        // Animation Event
        void OnAnimFrameHit() {
            if (target != null) {
                if (target.GetComponent<Health>().IsDead) {
                    Stop();
                }
                if (equippedWeapon.HasProjectile()) {
                     equippedWeapon.LaunchProjectile(handTransform, target.GetComponent<Health>(), gameObject);
                } else {
                    target.GetComponent<Health>().TakeDamage(gameObject, equippedWeapon.WeaponDamage);
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
                if (equippedWeapon) {
                    DestroyEquippedWeapon();
                }
                Weapon weapon = pickup.Weapon;
                equippedWeapon = weapon;
                equippedWeaponObject = weapon.Spawn(handTransform, GetComponent<Animator>());
                Destroy(pickup.gameObject);
            } else {
                Debug.LogError($"No weapon equipped to Fighter on {name}.");
            }
        }

        private void DestroyEquippedWeapon() {
            Destroy(equippedWeaponObject);
        }

        public object CaptureState()
        {
            return equippedWeapon.name;
        }

        public void RestoreState(object state)
        {
            EquipWeapon(Resources.Load<Weapon>(state as string));
        }
    }
}