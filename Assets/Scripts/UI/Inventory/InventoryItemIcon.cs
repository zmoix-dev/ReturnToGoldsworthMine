using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RPG.Inventories
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {

        private void Start() {
            if (GetItem() == null) {
                GetComponent<Image>().enabled = false;
            } else {
                GetComponent<Image>().enabled = true;
            }
        }

        // PUBLIC
        public void SetItem(InventoryItem item)
        {
            var iconImage = GetComponent<Image>();
            if (item == null)
            {
                iconImage.enabled = false;
            }
            else
            {
                iconImage.enabled = true;
                iconImage.sprite = item.GetIcon();
            }
        }

        public Sprite GetItem()
        {
            var iconImage = GetComponent<Image>();
            if (!iconImage.enabled)
            {
                return null;
            }
            return iconImage.sprite;
        }
    }
}