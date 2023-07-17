using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;
using RPG.Control;

namespace RPG.Combat {
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController caller)
        {
            if (gameObject.GetComponent<Health>() == null || gameObject.GetComponent<Health>().IsDead) return false;

            if (Input.GetMouseButtonDown(0)) {
                    caller.gameObject.GetComponent<Fighter>().SelectTarget(gameObject);
            }
            return true;
        }
    }
}