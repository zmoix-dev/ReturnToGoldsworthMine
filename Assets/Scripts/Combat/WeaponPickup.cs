using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] Weapon weapon;
        [SerializeField] float pickupRange = 100f;

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController caller)
        {
            if (weapon != null) {
                if (Input.GetMouseButtonDown(0)) {
                    if (Vector3.Distance(caller.gameObject.transform.position, transform.position) < pickupRange) {
                        caller.GetComponent<Fighter>().EquipWeapon(weapon);
                    }
                }
                return true;
            }
            return false;
        }

        void OnTriggerEnter(Collider other) {
            if (other.tag.Equals(UnitType.GetType(UnitType.Type.PLAYER))) {
                GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<Fighter>().EquipWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}