using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour
    {

        Fighter fighter;
        Mover mover;
        Health health;

        void Start() {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            if (health.IsDead) {
                this.enabled = false;
                fighter.enabled = false;
                mover.enabled = false;
            }
            
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            if (InteractWithWeapon(hits)) return;
            if (InteractWithCombat(hits)) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithWeapon(RaycastHit[] hits) {
            foreach (RaycastHit hit in hits) {
                WeaponPickup target = hit.transform.GetComponent<WeaponPickup>();
                if (!target) continue;
                GameObject targetObject = target.gameObject;
                if (Input.GetMouseButtonDown(0)) {
                    fighter.EquipWeapon(target);
                }
                return true;        
            }
            return false;
        }

        private bool InteractWithCombat(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits) {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (!target || target.GetComponent<Health>().IsDead) continue;
                GameObject targetObject = target.gameObject;
                if (Input.GetMouseButtonDown(0)) {
                    fighter.SelectTarget(targetObject);
                }
                return true;                
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            return MoveToCursor();
        }

        private bool MoveToCursor()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit && Input.GetMouseButton(0))
            {
                mover.StartMoveAction(hit.point);
                return true;
            } else {
                return false;
            }
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}