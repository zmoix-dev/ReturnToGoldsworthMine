using System;
using UnityEngine;
using RPG.Saving;
using Newtonsoft.Json.Linq;

namespace RPG.Inventories
{
    /// <summary>
    /// Provides storage for the player inventory. A configurable count of
    /// slots are available.
    ///
    /// This component should be placed on the GameObject tagged "Player".
    /// </summary>
    public class Inventory : MonoBehaviour, IJsonSaveable
    {
        // CONFIG DATA
        [Tooltip("Allowed size")]
        [SerializeField] int inventorySize = 16;

        // STATE
        InventorySlot[] slots;

        
        public struct InventorySlot {
            public InventorySlot(InventoryItem item, int count) {
                this.item = item;
                this.count = count;
            }
            public InventoryItem item;
            public int count;
        }

        [System.Serializable]
        private struct InventorySlotRecord {
            public InventorySlotRecord(string id, int count) {
                this.itemID = id;
                this.count = count;
            }
            public string itemID;
            public int count;
        }

        // PUBLIC

        /// <summary>
        /// Broadcasts when the items in the slots are added/removed.
        /// </summary>
        public event Action inventoryUpdated;

        /// <summary>
        /// Convenience for getting the player's inventory.
        /// </summary>
        public static Inventory GetPlayerInventory()
        {
            var player = GameObject.FindWithTag("Player");
            return player.GetComponent<Inventory>();
        }

        /// <summary>
        /// Could this item fit anywhere in the inventory?
        /// </summary>
        public bool HasSpaceFor(InventoryItem item, int count = 1)
        {
            //TODO no more space for stackables?
            return FindSlot(item) >= 0;
        }

        /// <summary>
        /// How many slots are in the inventory?
        /// </summary>
        public int GetSize()
        {
            return slots.Length;
        }

        public bool AddItem(InventoryItem item, int count) {
            int index = FindSlot(item);
            return AddItemToSlot(index, item, count);
        }

        /// <summary>
        /// Attempt to add the items to the first available slot.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>Whether or not the item could be added.</returns>
        public bool AddToFirstEmptySlot(InventoryItem item, int count)
        {
            int i = FindSlot(item);

            if (i < 0)
            {
                return false;
            }

            slots[i] = new InventorySlot(item, count);
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }

        /// <summary>
        /// Is there an instance of the item in the inventory?
        /// </summary>
        public int HasItem(InventoryItem item)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (object.ReferenceEquals(slots[i].item, item))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Return the item type in the given slot.
        /// </summary>
        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }

        /// <summary>
        /// Return the item count in the given slot.
        /// </summary>
        public int GetItemCountInSlot(int slot) {
            return slots[slot].count;
        }

        /// <summary>
        /// Remove the item from the given slot.
        /// </summary>
        public void RemoveFromSlot(int slot, int count)
        {
            slots[slot].count -= count;
            if (slots[slot].count <= 0) {
                slots[slot].item = null;
                slots[slot].count = 0;
            }
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }

        /// <summary>
        /// Will add an item to the given slot if possible. If there is already
        /// a stack of this type, it will add to the existing stack. Otherwise,
        /// it will be added to the first empty slot.
        /// </summary>
        /// <param name="slot">The slot to attempt to add to.</param>
        /// <param name="item">The item type to add.</param>
        /// <returns>True if the item was added anywhere in the inventory.</returns>
        public bool AddItemToSlot(int slot, InventoryItem item, int count)
        {
            if (slots[slot].item == null) {
                slots[slot] = new InventorySlot(item, count);
            }
            else if (slots[slot].item != null && slots[slot].item != item)
            {
                return AddToFirstEmptySlot(item, count);
            } else if (slots[slot].item == item) {
                if (item.IsStackable()) {
                    slots[slot].count += count;
                } else {
                    AddToFirstEmptySlot(item, count);
                }
            }
            
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }

        // PRIVATE

        private void Awake()
        {
            if (slots != null) return;
            slots = new InventorySlot[inventorySize];
        }

        /// <summary>
        /// Find a slot that can accomodate the given item.
        /// </summary>
        /// <returns>-1 if no slot is found.</returns>
        private int FindSlot(InventoryItem item)
        {
            int index = HasItem(item);
            if (index != -1) return index;

            return FindEmptySlot();
        }

        /// <summary>
        /// Find an empty slot.
        /// </summary>
        /// <returns>-1 if all slots are full.</returns>
        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public JToken CaptureAsJToken()
        {
            InventorySlotRecord[] slots = new InventorySlotRecord[this.slots.Length];
            for(int i = 0; i < this.slots.Length; i++) {
                if (this.slots[i].item == null) continue;
                slots[i] = new InventorySlotRecord(this.slots[i].item.GetItemID(), this.slots[i].count);
            }

            return JToken.FromObject(slots);
        }

        public void RestoreFromJToken(JToken state)
        {
            var slots = state.ToObject<InventorySlotRecord[]>();
            for (int i = 0; i < inventorySize; i++)
            {
                if (slots[i].itemID == string.Empty) continue;
                this.slots[i] = new InventorySlot(InventoryItem.GetFromID(slots[i].itemID), slots[i].count);
            }
            inventoryUpdated?.Invoke();
        }
    }
}