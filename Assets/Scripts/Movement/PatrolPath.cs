using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement {
    public class PatrolPath : MonoBehaviour
    {

        public Vector3 GetWaypoint(int index) {
            return transform.GetChild(index % transform.childCount).transform.position;
        }

        public int GetWaypointCount() {
            return transform.childCount;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < transform.childCount; i++) {
                Vector3 start = transform.GetChild(i).transform.position;
                Vector3 end = transform.GetChild((i + 1) % transform.childCount).transform.position;
                Gizmos.DrawLine(start, end);
            }
        }
    }
}
