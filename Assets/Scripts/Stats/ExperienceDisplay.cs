using RPG.Core;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
    public class ExperienceDisplay : MonoBehaviour {
    
        Experience experience;
        TextMeshProUGUI display;
        
        void Awake()
        {
            experience = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<Experience>();
            display = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            display.text = $"{experience.GetExperience()}";
        }

    }
}