using System;
using RPG.Combat;
using RPG.Inventories;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float navMeshHitRadius = 1f;

        Fighter fighter;
        Mover mover;
        Health health;

        bool isDraggingUI = false;

        [System.Serializable]
        struct CursorMapping {
            public CursorType type;
            public Vector2 hotspot;
            public Texture2D texture;
        }


        private void Awake() {
            fighter = GetComponent<Fighter>();
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (health.IsDead) {
                this.enabled = false;
                fighter.enabled = false;
                mover.enabled = false;
                SetCursor(CursorType.None);
                return;
            }

            CheckSpecialAbilityKeys();
            
            RaycastHit[] hits = GetRaycastSorted();
            // foreach (RaycastHit hit in hits) {
            //     Debug.Log(hit.transform.name);
            // }
            if (InteractWithUI(hits)) return;
            if (InteractWithComponent(hits)) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private void CheckSpecialAbilityKeys()
        {
            var actionStore = GetComponent<ActionStore>();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                actionStore.Use(0, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                actionStore.Use(1, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                actionStore.Use(2, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                actionStore.Use(3, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                actionStore.Use(4, gameObject);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                actionStore.Use(5, gameObject);
            }
        }

        RaycastHit[] GetRaycastSorted() {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++) {
                distances[i] = Vector3.Distance(hits[i].transform.position, transform.position);
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI(RaycastHit[] hits)
        {
            if (Input.GetMouseButtonUp(0)) {
                isDraggingUI = false;
            }
            if (EventSystem.current.IsPointerOverGameObject()) {
                if (Input.GetMouseButtonDown(0)) {
                    isDraggingUI = true;
                } 
                SetCursor(CursorType.UI);
                return true;
            }   
            if (isDraggingUI) {
                return true;
            }
            return false;
        }

        private bool InteractWithComponent(RaycastHit[] hits) {
            foreach (RaycastHit hit in hits) {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables) {
                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }     
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            Vector3 hit;
            bool hasHit = RaycastNavMesh(out hit);

            if (hasHit && mover.CanMoveTo(hit))
            {
                if (Input.GetMouseButton(0)) {
                    mover.StartMoveAction(hit);
                }
                SetCursor(CursorType.Movement);
                return true;
            } else {
                return false;
            }
        }

        private bool RaycastNavMesh(out Vector3 target) {
            target = Vector3.zero;
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasNavMeshHit = NavMesh.SamplePosition(hit.point, out navMeshHit, navMeshHitRadius, NavMesh.AllAreas);
            if (!hasNavMeshHit) return false;

            target = navMeshHit.position;
            return mover.CanMoveTo(target);
        }

        private Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
            
        }

        private CursorMapping GetCursorMapping(CursorType type) {
            foreach (CursorMapping mapping in cursorMappings) {
                if (mapping.type.Equals(type)) {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }
    }
}