using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventories;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.Stats {
    public class WeaponDamageDisplay : MonoBehaviour
    {
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
                equipment.equipmentUpdated += CalculateWeaponDamage;
            }
        }

        void Start() {
            CalculateWeaponDamage();
        }

        private void CalculateWeaponDamage()
        {
            float damage = stats.GetStat(StatsType.Damage);
            display.text = string.Format("{0:0}", damage);
        }
    }
}
