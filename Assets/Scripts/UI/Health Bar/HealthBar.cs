using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using UnityEngine;

namespace RPG.UI {
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] RectTransform currentHealthBar;
        [SerializeField] Canvas rootCanvas;

        void Update() {
            float healthPct = health.GetHealthPercentage();
            if (health.IsDead) {
                rootCanvas.enabled = false;
                return;
            }
            rootCanvas.enabled = true;
            currentHealthBar.localScale = new Vector3(healthPct, 1, 1);
        }
    }
}
