using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Game.Animation;

namespace RPG.Core {
    public class Health : MonoBehaviour
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
            GetComponent<ActionScheduler>().StopCurrentAction();
        }
    }
}