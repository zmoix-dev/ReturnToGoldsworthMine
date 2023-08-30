using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

public class HealthEquipmentUpdater : MonoBehaviour
{
    Equipment equipment;
    Health health;

    // STATE
    float maxHp;

    void Awake() {
        equipment = GetComponent<Equipment>();
        health = GetComponent<Health>();

        if (equipment) {
            equipment.equipmentUpdated += UpdateHealth;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHp = health.GetMaxHealth();
    }

    private void UpdateHealth() {
        float deltaHp = CalculateDeltaHp();
        if (deltaHp != 0) {
            health.RegenerateHealth(deltaHp);
            maxHp += deltaHp;
        }
    }

    private float CalculateDeltaHp() {
        return health.GetMaxHealth() - maxHp;
    }
}
