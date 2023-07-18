using System;
using RPG.Combat;
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
        [SerializeField] float maxNavPathLength = 40f;

        Fighter fighter;
        Mover mover;
        Health health;

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
            
            RaycastHit[] hits = GetRaycastSorted();
            // foreach (RaycastHit hit in hits) {
            //     Debug.Log(hit.transform.name);
            // }
            if (InteractWithUI(hits)) return;
            if (InteractWithComponent(hits)) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
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
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
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

            if (hasHit)
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

            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) {
                Debug.Log("No path");
                return false;
            }
            // if (path.status == NavMeshPathStatus.PathComplete) {
            //     Debug.Log("Incomplete path");
            //     return false;
            // }
            if (GetPathLength(path) > maxNavPathLength) {
                Debug.Log("Long path");
                return false;
            }

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float totalDistance = 0f;
            for (int i = 0; i < path.corners.Length - 1; i++) {
                totalDistance += Vector3.Distance(path.corners[i], path.corners[i+1]);
            }
            return totalDistance;
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