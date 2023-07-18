using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Core;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] UnitType.Type unitType;
        [SerializeField] Progression progression = null;
        [SerializeField] ParticleSystem levelUpVfx = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;

        Experience experience;

        private void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(GetInitialLevel);
        }

        private int GetInitialLevel() {
            if (unitType.Equals(UnitType.Type.PLAYER)) {
                return CalculateLevel();
            } else {
                return startingLevel;
            }
        }

        private void OnEnable() {
            if (experience) {
                experience.onExperienceGained += CheckLevel;
            }
        }

        private void OnDisable() {
            if (experience) {
                experience.onExperienceGained -= CheckLevel;
            }
        }

        private void Start() {
            currentLevel.ForceInit();
        }

        void CheckLevel() {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            ParticleSystem vfxDestroy = Instantiate(levelUpVfx, transform);
            Destroy(vfxDestroy, 1f);
        }

        public float GetStat(StatsType stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifiers(stat)) * GetPercentageModifiers(stat);
        }

        public float GetExperienceReward() {
            return 10;
        }

        public int GetLevel() {
            if (currentLevel.value < 1) {
                currentLevel.value = CalculateLevel();
            }
            return currentLevel.value;
        }

        public int CalculateLevel() {
            if (experience == null) return 1;
            float currentExp = experience.GetExperience();
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

        private float GetBaseStat(StatsType stat)
        {
            return progression.GetStat(unitType, stat, currentLevel.value);
        }

        private float GetBaseStat(StatsType stat, int level)
        {
            return progression.GetStat(unitType, stat, level);
        }

        private float GetAdditiveModifiers(StatsType stat) {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetAdditiveModifiers(stat)) {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifiers(StatsType stat) {
            if (!shouldUseModifiers) return 1;
            float total = 1;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in provider.GetPercentageModifiers(stat)) {
                    total += modifier;
                }
            }
            return total;
        }
    }
}
