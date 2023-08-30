using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.Inventories {
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(StatsType stat)
        {
            foreach (var slot in GetAllPopulatedSlots()) {
                var item = GetItemInSlot(slot) as IModifierProvider;

                if (item == null) continue;
                foreach(float modifier in item.GetAdditiveModifiers(stat)) {
                    yield return modifier;
                }
            }
        }

        public IEnumerable<float> GetPercentageModifiers(StatsType stat)
        {
            foreach (var slot in GetAllPopulatedSlots()) {
                var item = GetItemInSlot(slot) as IModifierProvider;

                if (item == null) continue;
                foreach(float modifier in item.GetPercentageModifiers(stat)) {
                    yield return modifier;
                }
            }
        }
    }

}
