using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat {
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] WeaponConfig weapon;
        [SerializeField] float pickupRange = 100f;
        [SerializeField] float healthRegen = 0f;
        [SerializeField] float respawnTime = 5f;

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        private void Pickup(GameObject initiator) {
            Fighter fighter = initiator.GetComponent<Fighter>();
            Health health = initiator.GetComponent<Health>();
            if (fighter != null && weapon != null) {
                fighter.EquipWeapon(weapon);
            }
            if (health != null && healthRegen != 0) {
                health.RegenerateHealth(healthRegen);
            }
            StartCoroutine(TogglePickupVisibility());
        }

        private IEnumerator TogglePickupVisibility() {
            ShouldShow(false);
            yield return new WaitForSeconds(respawnTime);
            ShouldShow(true);
        }

        private void ShouldShow(bool shouldShow) {
            GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform) {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
}