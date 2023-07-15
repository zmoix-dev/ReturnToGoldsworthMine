using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Game.Animation;
using RPG.Saving;

namespace RPG.Core {
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float maxHealth = 100f;
        [SerializeField] float currentHealth;
        bool isDead = false;
        public bool IsDead { get { return isDead; }}

        void Start() {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage) {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth == 0 && !isDead)
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger(AnimationStates.DEAD);
            if (GetComponent<Rigidbody>()) {
                GetComponent<Rigidbody>().isKinematic = true;
            }
            GetComponent<ActionScheduler>().StopCurrentAction();
        }

        public object CaptureState()
        {
            return currentHealth;
        }

        public void RestoreState(object state)
        {
            currentHealth = (float) state;
            if (currentHealth == 0) {
                HandleDeath();
            }
        }
    }
}