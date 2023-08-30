using System.Collections.Generic;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Analytics;

namespace RPG.Combat {
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG/Weapons/New Weapon", order = 0)]
    public class WeaponConfig: EquipableItem, IModifierProvider {

        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [Range(0, 1)]
        [SerializeField] float pctModifer = 0f;
        [SerializeField] float attackRange = 3.5f;
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] Projectile projectile = null;

        public float WeaponDamage { get { return weaponDamage; }}
        public float PctModifier { get { return pctModifer; }}
        public float AttackRange { get { return attackRange; }}
        public float TimeBetweenAttacks { get { return timeBetweenAttacks; }}

        public Weapon Spawn(Transform handTransform, Animator animator, Quaternion reposition, Vector3 weaponScale) {
            Weapon weapon = null;
            if (weaponPrefab) {
                weapon = Instantiate(weaponPrefab, handTransform);
                weapon.gameObject.transform.Rotate(reposition.eulerAngles);
                weapon.gameObject.transform.localScale = weaponScale;
            }
            if (animatorOverride) {
                animator.runtimeAnimatorController = animatorOverride;
            } else {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if (overrideController) {
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                }
            }
            return weapon;
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public void LaunchProjectile(Transform hand, Health target, GameObject attacker, float damageModifier) {
            Projectile projectileInstance = Instantiate(projectile, hand.position, Quaternion.identity);
            projectileInstance.SetTarget(target, attacker, weaponDamage);
        }

        public IEnumerable<float> GetAdditiveModifiers(StatsType stat)
        {
            if (stat.Equals(StatsType.Damage)) {
                yield return weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(StatsType stat)
        {
            if (stat.Equals(StatsType.Damage)) {
                yield return pctModifer;
            }
        }
    }
}