using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the count.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        // STATE
        InventoryItem item;
        int count;

        // CACHED REFERENCE
        Inventory inventory;

        // LIFECYCLE METHODS

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        // PUBLIC

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        public void Setup(InventoryItem item, int count)
        {
            this.item = item;
            this.count = count;
        }

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetCount() {
            return count;
        }

        public void PickupItem()
        {
            bool foundSlot = inventory.AddItem(item, count);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(item, count);
        }
    }
}