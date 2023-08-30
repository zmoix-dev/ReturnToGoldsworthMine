using System.Collections;
using System.Collections.Generic;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories {
    [CreateAssetMenu(menuName = ("RPG/Inventory/EquipableItem"))]
    public class StatsEquipable : EquipableItem, IModifierProvider
    {
        [SerializeField] Modifier[] additiveModifiers;
        [SerializeField] Modifier[] percentageModifiers;

        [System.Serializable]
        struct Modifier {
            public StatsType stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifiers(StatsType stat)
        {
            foreach(Modifier mod in additiveModifiers) {
                if (!mod.stat.Equals(stat)) continue;
                yield return mod.value;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(StatsType stat)
        {
            foreach(Modifier mod in percentageModifiers) {
                if (!mod.stat.Equals(stat)) continue;
                yield return mod.value;
            }
        }
    }
}
