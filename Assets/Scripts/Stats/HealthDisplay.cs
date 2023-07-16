using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Stats {
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        TextMeshProUGUI display;
        
        void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            display = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            display.text = string.Format("{0:0}%", health.GetHealthPercentage());
        }
    }
}