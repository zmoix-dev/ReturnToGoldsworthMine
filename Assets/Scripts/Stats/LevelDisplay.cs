using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
    public class LevelDisplay : MonoBehaviour {
    
        BaseStats baseStats;
        TextMeshProUGUI display;
        
        void Awake()
        {
            baseStats = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<BaseStats>();
            display = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            display.text = $"{baseStats.CalculateLevel()}";
        }

    }
}
