using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI {    
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text damageText = null;
        public void SetValue(float damage) {
            damageText.text = string.Format("{0:0.0}", damage);
        }
    }
}