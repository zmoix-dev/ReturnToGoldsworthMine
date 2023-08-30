using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
    public class StatDisplay : MonoBehaviour
    {
        [SerializeField] StatsType stat;
        BaseStats stats;
        Equipment equipment;
        TextMeshProUGUI display;

        // Start is called before the first frame update
        void Awake()
        {
            GameObject player = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER));
            stats = player.GetComponent<BaseStats>();
            equipment = player.GetComponent<StatsEquipment>();
            display = GetComponent<TextMeshProUGUI>();

            if (equipment) {
                equipment.equipmentUpdated += CalculateStat;
            }
        }

        void Start() {
            CalculateStat();
        }

        private void CalculateStat()
        {
            float myStat = stats.GetStat(stat);
            display.text = string.Format("{0:0}", myStat);
        }
    }
}

