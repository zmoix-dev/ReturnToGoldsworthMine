using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

namespace RPG.Inventories
{
    /// <summary>
    /// To be put on the icon representing an inventory item. Allows the slot to
    /// update the icon and number.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] GameObject countContainer = null;
        [SerializeField] TextMeshProUGUI itemCount = null;

        private void Start() {
            if (GetItem() == null) {
                GetComponent<Image>().enabled = false;
                ToggleStackCountDisplay(false);
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

        public void SetCount(int count) {
            if (!countContainer || !itemCount) return;
            if (count <= 1) {
                ToggleStackCountDisplay(false);
                return;
            }
            ToggleStackCountDisplay(true);
            itemCount.text = count.ToString();
        }

        public int GetCount() {
            if (itemCount) {
                return int.Parse(itemCount.text);
            }
            return 1;
        }

        // PRIVATE METHODS

        private void ToggleStackCountDisplay(bool toggle) {
            if (!countContainer || !itemCount) return;
            countContainer.SetActive(toggle);
            itemCount.enabled = toggle;
        }
    }
}