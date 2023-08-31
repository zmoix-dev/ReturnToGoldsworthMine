using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine.Playables;
using RPG.Core.Saving;
using UnityEngine.SceneManagement;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed on anything that wishes to drop pickups into the world.
    /// Tracks the drops for saving and restoring.
    /// </summary>
    public class ItemDropper : MonoBehaviour, IJsonSaveable
    {
        // STATE
        private List<Pickup> droppedItems = new List<Pickup>();
        private List<DropRecord> otherSceneDroppedItems = new List<DropRecord>();

        // PUBLIC

        /// <summary>
        /// Create a pickup at the current position.
        /// </summary>
        /// <param name="item">The item type for the pickup.</param>
        public void DropItem(InventoryItem item, int count)
        {
            SpawnPickup(item, count, GetDropLocation());
        }

        // PROTECTED

        /// <summary>
        /// Override to set a custom method for locating a drop.
        /// </summary>
        /// <returns>The location the drop should be spawned.</returns>
        protected virtual Vector3 GetDropLocation()
        {
            return transform.position;
        }

        // PRIVATE

        public void SpawnPickup(InventoryItem item, int count, Vector3 spawnLocation)
        {
            var pickup = item.SpawnPickup(spawnLocation, count);
            droppedItems.Add(pickup);
        }

        [System.Serializable]
        private struct DropRecord
        {
            public string itemID;
            public int count;
            public SerializableVector3 position;
            public int sceneBuildIndex;
        }

        public JToken CaptureAsJToken()
        {
            Debug.Log("Capturing Dropped Items.");
            RemoveDestroyedDrops();
            var droppedItemsList = new List<DropRecord>();
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            foreach (Pickup pickup in droppedItems) {
                var droppedItem = new DropRecord();
                droppedItem.itemID = pickup.GetItem().GetItemID();
                droppedItem.count = pickup.GetCount();
                SerializableVector3 v3 = new SerializableVector3(pickup.gameObject.transform.position);
                droppedItem.position = v3;
                droppedItem.sceneBuildIndex = buildIndex;
                droppedItemsList.Add(droppedItem);
            }
            droppedItemsList.AddRange(otherSceneDroppedItems);
            Debug.Log($"Dropped items stored: {droppedItemsList.Count}");
            return JToken.FromObject(droppedItemsList);
        }

        public void RestoreFromJToken(JToken state)
        {
            Debug.Log("Restoring Dropped Items.");
            var droppedItemsList = state.ToObject<List<DropRecord>>();
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            otherSceneDroppedItems.Clear();
            Debug.Log($"Items to restore: {droppedItemsList.Count}");
            foreach (var item in droppedItemsList) {
                Debug.Log($"Checking items for scene {buildIndex}.");
                if (item.sceneBuildIndex.Equals(buildIndex)) {
                    Debug.Log("Items belong in scene.");
                    var pickupItem = InventoryItem.GetFromID(item.itemID);
                    SpawnPickup(pickupItem, item.count, item.position.ToVector());
                } else {
                    Debug.Log("Items do not belong in scene.");
                    otherSceneDroppedItems.Add(item);
                }
            }
        }

        /// <summary>
        /// Remove any drops in the world that have subsequently been picked up.
        /// </summary>
        private void RemoveDestroyedDrops()
        {
            var newList = new List<Pickup>();
            foreach (var item in droppedItems)
            {
                if (item != null)
                {
                    newList.Add(item);
                }
            }
            droppedItems = newList;
        }
    }
}