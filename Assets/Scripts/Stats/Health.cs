using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Game.Animation;
using RPG.Saving;
using RPG.Core;

namespace RPG.Stats {
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float currentHealth;
        bool isDead = false;
        public bool IsDead { get { return isDead; }}

        void Start() {
            currentHealth = GetComponent<BaseStats>().GetHealth();
        }

        public float GetHealthPercentage() {
            return 100 * (currentHealth / GetComponent<BaseStats>().GetHealth());
        }

        public void TakeDamage(GameObject attacker, float damage) {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth == 0 && !isDead)
            {
                HandleDeath(attacker);
            }
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