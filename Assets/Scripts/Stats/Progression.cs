using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/Progression", order = 0)]
    public class Progression : ScriptableObject {
        [SerializeField] CharacterProgression[] characterProgression = null;
        Dictionary<UnitType.Type, Dictionary<StatsType, float[]>> lookupTable = null;

        public float GetStat(UnitType.Type unitType, StatsType type, int level) {
            BuildLookup();

            if (!lookupTable.ContainsKey(unitType)) return -1;
            if (!lookupTable[unitType].ContainsKey(type)) return -1;
            if (lookupTable[unitType][type].Length < level) return -1;

            return lookupTable[unitType][type][level - 1];
        }

        public void BuildLookup() {
            if (lookupTable != null) return;
            lookupTable = new Dictionary<UnitType.Type, Dictionary<StatsType, float[]>>();
            foreach (CharacterProgression charProg in characterProgression) {
                lookupTable.Add(charProg.unitType, new Dictionary<StatsType, float[]>());
                foreach (CharacterStat stat in charProg.stats) {
                    lookupTable[charProg.unitType].Add(stat.statsType, stat.values);
                }
            }
        }

        [System.Serializable]
        class CharacterProgression {
            [SerializeField] public UnitType.Type unitType;
            [SerializeField] public CharacterStat[] stats;
        }

        [System.Serializable]
        class CharacterStat {
            [SerializeField] public StatsType statsType;
            [SerializeField] public float[] values;
        }
    }
}