using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Stats;
using RPG.Core;

namespace RPG.Combat {
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter playerFighter;
        TextMeshProUGUI display;
        
        void Awake()
        {
            playerFighter = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<Fighter>();
            display = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            GameObject target = playerFighter.GetTarget();
            string text = null;
            if (target == null) {
                text = "N/A";
            } else {
                text = string.Format("{0:0}%", target.GetComponent<Health>().GetHealthPercentage());
            }
            display.text = text;
        }
    }
}