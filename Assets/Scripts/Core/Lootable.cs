using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    GameObject player;

    void Start(){ 
        player = GameObject.FindWithTag(UnitType.GetUnitType(UnitType.UnitTypes.PLAYER));
    }
}
