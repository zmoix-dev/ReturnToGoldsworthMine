using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Game.Animation;
using RPG.Saving;
using RPG.Core;

namespace RPG.Stats {
    public class Health : MonoBehaviour, ISaveable
    {
        [Range(0,1)]
        [SerializeField] float levelUpMinHealthPct = 0.75f;
        float currentHealth = -1f;
        bool isDead = false;
        public bool IsDead { get { return isDead; }}

        void Start() {
            // Fixes race condition with RestoreState
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
            if (currentHealth < 0) {
                currentHealth = GetComponent<BaseStats>().GetStat(StatsType.Health);
            }
            
        }

        public float GetHealthPercentage() {
            return 100 * (currentHealth / GetComponent<BaseStats>().GetStat(StatsType.Health));
        }

        public void TakeDamage(GameObject attacker, float damage) {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth == 0 && !isDead)
            {
                HandleDeath(attacker);
            }
        }

        public void RegenerateHealth() {
            currentHealth = Mathf.Max(GetComponent<BaseStats>().GetStat(StatsType.Health) * levelUpMinHealthPct, currentHealth);
        }

        private void HandleDeath(GameObject attacker)
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger(AnimationStates.DEAD);
            if (GetComponent<Rigidbody>()) {
                GetComponent<Rigidbody>().isKinematic = true;
            }
            GetComponent<ActionScheduler>().StopCurrentAction();

            if (attacker && attacker.GetComponent<Experience>()) {
                float experience = GetComponent<BaseStats>().GetExperienceReward();
                attacker.GetComponent<Experience>().GrantExperience(experience);    
            }
            
        }

        public object CaptureState()
        {
            return currentHealth;
        }

        public void RestoreState(object state)
        {
            currentHealth = (float) state;
            if (currentHealth == 0) {
                HandleDeath(null);
            }
        }
    }
}