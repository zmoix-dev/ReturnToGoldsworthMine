using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Game.Animation;
using RPG.Saving;
using RPG.Core;
using UnityEngine.Events;
using GameDevTV.Utils;
using System;
using Newtonsoft.Json.Linq;

namespace RPG.Stats {
    public class Health : MonoBehaviour, IJsonSaveable
    {
        [Range(0,1)]
        [SerializeField] float levelUpMinHealthPct = 1f;
        [SerializeField] UnityEvent<float, GameObject> onTakeDamage;
        [SerializeField] UnityEvent onDie;
        LazyValue<float> currentHealth;
        bool isDead = false;
        bool wasHitByPlayer = false;
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

        public float GetCurrentHealth() {
            return currentHealth.value;
        }

        public float GetMaxHealth() {
            return GetComponent<BaseStats>().GetStat(StatsType.Health);
        }

        public void TakeDamage(GameObject attacker, float damage) {
            currentHealth.value = Mathf.Max(currentHealth.value - damage, 0);
            onTakeDamage.Invoke(damage, attacker);
            if (attacker.gameObject.tag.Equals(UnitType.GetType(UnitType.Type.PLAYER))) {
                wasHitByPlayer = true;
            }
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
            currentHealth.value = Mathf.Min(currentHealth.value + value, GetComponent<BaseStats>().GetStat(StatsType.Health));
        }

        private void HandleDeath(GameObject attacker)
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger(AnimationStates.DEAD);
            if (GetComponent<Rigidbody>()) {
                GetComponent<Rigidbody>().isKinematic = true;
            }
            GetComponent<ActionScheduler>().StopCurrentAction();

            if (wasHitByPlayer) {
                float experience = GetComponent<BaseStats>().GetExperienceReward();
                GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<Experience>().GrantExperience(experience);    
            }
            
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentHealth.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            currentHealth.value = state.ToObject<float>();
            if (currentHealth.value == 0) {
                HandleDeath(null);
            }
        }
    }
}