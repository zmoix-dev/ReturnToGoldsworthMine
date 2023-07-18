using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using RPG.Game.Animation;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] Transform handTransform;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] Quaternion weaponRotation = Quaternion.identity;
        [SerializeField] Vector3 weaponScale = Vector3.one;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        GameObject equippedWeaponObject;
       
        GameObject target = null;
        bool canAttack = true;
        Mover mover;

        private void Awake() {
            mover = GetComponent<Mover>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupCurrentWeapon);
        }

        private Weapon SetupCurrentWeapon()
        {
            return EquipWeapon(currentWeaponConfig, weaponRotation, weaponScale);
        }

        private void Start() {
            currentWeapon.ForceInit();
        }

        private void Update() {
            ChaseTarget();
        }

        public void Stop() {
            this.target = null;
            GetComponent<Animator>().ResetTrigger(AnimationStates.ATTACK);
            GetComponent<Animator>().SetTrigger(AnimationStates.STOP_ATTACK);
            StopAllCoroutines();
            canAttack = true;
        }

        public IEnumerable<float> GetAdditiveModifiers(StatsType stat)
        {
            if (stat.Equals(StatsType.Damage)) {
                yield return currentWeaponConfig.WeaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(StatsType stat)
        {
            if (stat.Equals(StatsType.Damage)) {
                yield return currentWeaponConfig.PctModifier;
            }
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
            if (target == null) return;
            if (target.GetComponent<Health>().IsDead) {
                Stop();
                return;
            }

            float damageCalc = GetComponent<BaseStats>().GetStat(StatsType.Damage);
            if (currentWeapon.value != null) {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile()) {
                    currentWeaponConfig.LaunchProjectile(handTransform, target.GetComponent<Health>(), gameObject, damageCalc);
            } else {
                target.GetComponent<Health>().TakeDamage(gameObject, currentWeaponConfig.WeaponDamage * damageCalc);
            }
        }

        private void ChaseTarget()
        {
            if (CanChase(target)) {
                if (Vector3.Distance(transform.position, target.transform.position) <= currentWeaponConfig.AttackRange)
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

        public bool CanChase(GameObject combatTarget) {
            if (combatTarget == null) return false;
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position)) return false;
            Health enemyHealth = combatTarget.GetComponent<Health>();
            return enemyHealth != null && !enemyHealth.IsDead;
        }

        private IEnumerator AttackBehavior()
        {
            transform.LookAt(target.transform);
            GetComponent<Animator>().ResetTrigger(AnimationStates.STOP_ATTACK);
            GetComponent<Animator>().SetTrigger(AnimationStates.ATTACK);
            canAttack = false;
            yield return new WaitForSeconds(currentWeaponConfig.TimeBetweenAttacks);
            canAttack = true;
        }

        public Weapon EquipWeapon(WeaponConfig weapon, Quaternion reposition, Vector3 weaponScale)
        {
            if (weapon) {
                currentWeaponConfig = weapon;
                return weapon.Spawn(handTransform, GetComponent<Animator>(), reposition, weaponScale);
            } else {
                Debug.LogError($"No weapon equipped to Fighter on {name}.");
                return null;
            }
        }

        public Weapon EquipWeapon(WeaponConfig weapon)
        {
            return EquipWeapon(weapon, weaponRotation, weaponScale);
        }

        private void DestroyEquippedWeapon() {
            Destroy(equippedWeaponObject);
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            EquipWeapon(Resources.Load<WeaponConfig>(state as string), weaponRotation, weaponScale);
        }
    }
}