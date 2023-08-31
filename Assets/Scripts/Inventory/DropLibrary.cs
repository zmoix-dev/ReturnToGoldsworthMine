using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using UnityEngine;

namespace RPG.Inventories {
    [CreateAssetMenu(menuName = ("RPG/Inventory/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        [SerializeField] float dropChancePercentage;
        [SerializeField] int minDrops;
        [SerializeField] int maxDrops;
        [SerializeField] DropConfig[] potentialDrops;
    
        [System.Serializable]
        class DropConfig {
            public InventoryItem item;
            public float relativeChance;
            public int minCount;
            public int maxCount;
            public int GetRandomCount() {
                if (!item.IsStackable()) return 1;
                return UnityEngine.Random.Range(minCount, maxCount + 1);
            }
        }

        public struct Dropped {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops() {
            if (!ShouldRandomDrop()) yield break;
            for (int i = 0; i < GetNumberOfRandomDrops(); i++) {
                yield return GetRandomDrop();
            }
        }

        private bool ShouldRandomDrop()
        {
            // Generate random 1-100 inclusive
            float chance = UnityEngine.Random.Range(0, 100) + 1;
            return chance <= dropChancePercentage;
        }

        private int GetNumberOfRandomDrops()
        {
            return UnityEngine.Random.Range(minDrops, maxDrops + 1);
        }

        private Dropped GetRandomDrop()
        {
            var drop = SelectRandomItem();
            var result = new Dropped();
            result.item = drop.item;
            result.number = drop.GetRandomCount();
            return result;
        }

        private DropConfig SelectRandomItem() {
            float totalChance = GetTotalChance();
            float randomRoll = UnityEngine.Random.Range(0, totalChance);
            float chanceRunningTotal = 0;
            foreach (var drop in potentialDrops) {
                chanceRunningTotal += drop.relativeChance;
                if (randomRoll <= chanceRunningTotal) {
                    return drop;
                }
            }
            return null;
        }

        private float GetTotalChance()
        {
            float total = 0;
            foreach (var drop in potentialDrops) {
                total += drop.relativeChance;
            }
            return total;
        }
    }
}