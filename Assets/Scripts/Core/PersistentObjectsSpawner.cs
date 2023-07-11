using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class PersistentObjectsSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectsPrefab;

        static bool hasSpawned = false;

        private void Awake() {
            if (!hasSpawned) {
                SpawnPersistentObjects();
                hasSpawned = true;
            }
        }

        private void SpawnPersistentObjects()
        {
            GameObject instance = Instantiate(persistentObjectsPrefab);
            DontDestroyOnLoad(instance);
        }
    }
}