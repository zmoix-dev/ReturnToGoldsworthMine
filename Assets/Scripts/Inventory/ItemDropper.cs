using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using UnityEngine.Playables;
using RPG.Core.Saving;

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
        }

        public JToken CaptureAsJToken()
        {
            RemoveDestroyedDrops();
            var droppedItemsList = new DropRecord[droppedItems.Count];
            for (int i = 0; i < droppedItemsList.Length; i++) {
                droppedItemsList[i].itemID = droppedItems[i].GetItem().GetItemID();
                droppedItemsList[i].count = droppedItems[i].GetCount();
                SerializableVector3 v3 = new SerializableVector3(droppedItems[i].gameObject.transform.position);
                Debug.Log($"{v3.x},{v3.y},{v3.z}");
                droppedItemsList[i].position = v3;
            }
            return JToken.FromObject(droppedItemsList);
        }

        public void RestoreFromJToken(JToken state)
        {
            var droppedItemsList = state.ToObject<DropRecord[]>();
            foreach (var item in droppedItemsList) {
                var pickupItem = InventoryItem.GetFromID(item.itemID);
                SpawnPickup(pickupItem, item.count, item.position.ToVector());
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