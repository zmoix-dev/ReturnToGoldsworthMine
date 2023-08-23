using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RPG.UI.Dragging;
using RPG.Inventories;
using RPG.UI.Inventories;

namespace RPG.Inventories
{
    public class InventorySlotUI : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        // CONFIG DATA
        [SerializeField] InventoryItemIcon icon = null;

        // STATE
        int index;
        Inventory inventory;

        // PUBLIC

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            InventoryItem item = inventory.GetItemInSlot(index);
            int count = inventory.GetItemCountInSlot(index);
            if (item) {
                icon.SetItem(item);
                icon.SetCount(count);
            }
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (inventory.HasSpaceFor(item))
            {
                return int.MaxValue;
            }
            return 0;
        }

        public void AddItems(InventoryItem item, int count)
        {
            inventory.AddItemToSlot(index, item, count);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int GetCount()
        {
            return 1;
        }

        public void RemoveItems(int count)
        {
            inventory.RemoveFromSlot(index, count);
        }
    }
}