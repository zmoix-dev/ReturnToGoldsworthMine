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

        public int GetLevel() {
            float currentExp = GetComponent<Experience>().GetExperience();
            float expNeeded = -1;
            int level = 1;
            do {
                expNeeded = progression.GetStat(UnitType.Type.PLAYER, StatsType.ExperienceToLevelUp, level);
                if (currentExp < expNeeded) {
                   return level;
                }
                level++;
            } while (expNeeded != -1);
            return --level;
        }
    }
}
