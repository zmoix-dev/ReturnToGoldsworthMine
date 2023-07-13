using UnityEngine;

namespace RPG.Combat {
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon: ScriptableObject {

        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float attackRange = 3.5f;
        [SerializeField] float timeBetweenAttacks = 3f;

        public float WeaponDamage { get { return weaponDamage; }}
        public float AttackRange { get { return attackRange; }}
        public float TimeBetweenAttacks { get { return timeBetweenAttacks; }}

        public void Spawn(Transform handTransform, Animator animator) {
            if (weaponPrefab) {
                Instantiate(weaponPrefab, handTransform);
                if (animatorOverride) {
                    animator.runtimeAnimatorController = animatorOverride;
                }
            }
        }
    }
}