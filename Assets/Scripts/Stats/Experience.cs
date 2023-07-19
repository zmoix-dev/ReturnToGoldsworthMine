using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats {
    public class Experience : MonoBehaviour, IJsonSaveable {
        [SerializeField] float experiencePoints = 0;
    
        public event Action onExperienceGained;

        public void GrantExperience(float experience){ 
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperience() {
            return experiencePoints;
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state)
        {
            experiencePoints = state.ToObject<float>();
        }
    }
}