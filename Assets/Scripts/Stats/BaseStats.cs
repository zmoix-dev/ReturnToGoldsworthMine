using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] UnitType.Type unitType;
        [SerializeField] Progression progression = null;

        public float GetHealth() {
            return progression.GetStat(unitType, StatsType.Health, startingLevel);
        }

        public float GetExperienceReward() {
            return 10;
        }
    }
}
