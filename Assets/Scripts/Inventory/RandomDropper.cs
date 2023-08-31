using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Inventories {
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far away from the dropper the pickups can be scattered.")]
        [SerializeField] float scatterDistance = 1f;
        [SerializeField] DropLibrary dropLibrary;

        Health health;
        
        const int ATTEMPTS = 10;

        void Awake() {
            health = GetComponent<Health>();
        }

        public void RandomDrop() {
            var drops = dropLibrary.GetRandomDrops();
            foreach (var drop in drops) {
                DropItem(drop.item, drop.number);
            }
            
        }

        protected override Vector3 GetDropLocation() {
            for (int i = 0 ; i < ATTEMPTS; i++) {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas)) {
                    return hit.position;
                }
            }
            return transform.position;
        }
    }
}
