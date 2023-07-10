using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement {
    public class Waypoint : MonoBehaviour
    {
        const float waypointRadius = 0.33f;
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, waypointRadius);
        }
    }
}