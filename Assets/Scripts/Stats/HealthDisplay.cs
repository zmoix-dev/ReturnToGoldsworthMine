using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        TextMeshProUGUI display;

        // Start is called before the first frame update
        void Awake()
        {
            health = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<Health>();
            display = GetComponent<TextMeshProUGUI>();
        }

        void Update() {
            CalculateStat();
        }

        void CalculateStat()
        {
            float currentHp = health.GetCurrentHealth();
            float maxHp = health.GetMaxHealth();
            display.text = $"{currentHp} / {maxHp}";
        }
    }
}