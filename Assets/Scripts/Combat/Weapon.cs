using RPG.Core;
using UnityEngine;

namespace RPG.Combat {
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon: ScriptableObject {

        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float attackRange = 3.5f;
        [SerializeField] float timeBetweenAttacks = 3f;
        [SerializeField] Projectile projectile = null;

        public float WeaponDamage { get { return weaponDamage; }}
        public float AttackRange { get { return attackRange; }}
        public float TimeBetweenAttacks { get { return timeBetweenAttacks; }}

        public GameObject Spawn(Transform handTransform, Animator animator) {
            GameObject weapon = null;
            if (weaponPrefab) {
                weapon = Instantiate(weaponPrefab, handTransform);
            }
            if (animatorOverride) {
                animator.runtimeAnimatorController = animatorOverride;
            }
            return weapon;
        }

        public bool HasProjectile() {
            return projectile != null;
        }

        public void LaunchProjectile(Transform hand, Health target) {
            Projectile projectileInstance = Instantiate(projectile, hand.position, Quaternion.identity);
            projectileInstance.SetTarget(target, weaponDamage);
        }
    }
}