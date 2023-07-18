using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Game.Animation;
using RPG.Saving;
using RPG.Core;
using UnityEngine.Events;
using GameDevTV.Utils;
using System;

namespace RPG.Stats {
    public class Health : MonoBehaviour, ISaveable
    {
        [Range(0,1)]
        [SerializeField] float levelUpMinHealthPct = 1f;
        [SerializeField] UnityEvent<float> onTakeDamage;
        [SerializeField] UnityEvent onDie;
        LazyValue<float> currentHealth;
        bool isDead = false;
        public bool IsDead { get { return isDead; }}

        private void Awake() {
            currentHealth = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(StatsType.Health);
        }

        private void OnEnable() {

            GetComponent<BaseStats>().onLevelUp += RegenerateHealthOnLevelUp;  
        }

        private void OnDisable() {

            GetComponent<BaseStats>().onLevelUp -= RegenerateHealthOnLevelUp;  
        }

        private void Start() {
            currentHealth.ForceInit();
        }

        public float GetHealthPercentage() {
            return currentHealth.value / GetComponent<BaseStats>().GetStat(StatsType.Health);
        }

        public void TakeDamage(GameObject attacker, float damage) {
            currentHealth.value = Mathf.Max(currentHealth.value - damage, 0);
            onTakeDamage.Invoke(damage);
            if (currentHealth.value == 0 && !isDead)
            {
                onDie.Invoke();
                HandleDeath(attacker);
            }
        }

        public void RegenerateHealthOnLevelUp() {
            currentHealth.value = Mathf.Max(GetComponent<BaseStats>().GetStat(StatsType.Health) * levelUpMinHealthPct, currentHealth.value);
        }

        public void RegenerateHealth(float value) {
            currentHealth.value += value;
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
            currentHealth.value = (float) state;
            if (currentHealth.value == 0) {
                HandleDeath(null);
            }
        }
    }
}