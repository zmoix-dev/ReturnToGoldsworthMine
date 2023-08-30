using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Game.Animation;
using RPG.Inventories;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction, IJsonSaveable
    {
        [SerializeField] Transform handTransform;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] Quaternion weaponRotation = Quaternion.identity;
        [SerializeField] Vector3 weaponScale = Vector3.one;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        GameObject equippedWeaponObject;
        Equipment equipment;
       
        GameObject target = null;
        bool canAttack = true;
        Mover mover;

        private void Awake() {
            mover = GetComponent<Mover>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupCurrentWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment) {
                equipment.equipmentUpdated += UpdatedWeapon;
            }
        }

        private void UpdatedWeapon()
        {
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            DestroyEquippedWeapon();
            EquipWeapon(weapon);
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
                target.GetComponent<Health>().TakeDamage(gameObject, damageCalc);
            }
        }

        private void ChaseTarget()
        {
            if (CanChase(target))
            {
                if (IsInRange(target.transform))
                {
                    mover.Stop();
                    if (canAttack)
                    {
                        StartCoroutine(AttackBehavior());
                    }
                }
                else
                {
                    mover.MoveTo(target.transform.position);
                    GetComponent<Animator>().SetTrigger(AnimationStates.STOP_ATTACK);
                }
            }
        }

        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.AttackRange;
        }

        public bool CanChase(GameObject combatTarget) {
            if (combatTarget == null) return false;
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !IsInRange(combatTarget.transform)) {
                    return false;
            } 
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
                Weapon spawnedWeapon = weapon.Spawn(handTransform, GetComponent<Animator>(), reposition, weaponScale);
                if (spawnedWeapon != null) {
                    equippedWeaponObject = spawnedWeapon.gameObject;
                } else {
                    DestroyEquippedWeapon();
                }
                return spawnedWeapon;
            } else {
                currentWeaponConfig = null;
                EquipWeapon(defaultWeapon);
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

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            EquipWeapon(Resources.Load<WeaponConfig>(state.ToObject<string>()), weaponRotation, weaponScale);
        }
    }
}