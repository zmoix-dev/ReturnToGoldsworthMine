using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon;
        public Weapon Weapon { get { return weapon; }}
        void OnTriggerEnter(Collider other) {
            if (other.tag.Equals(UnitType.GetType(UnitType.Type.PLAYER))) {
                GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<Fighter>().EquipWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}