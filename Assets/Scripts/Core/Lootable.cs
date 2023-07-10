using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    GameObject player;

    void Start(){ 
        player = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER));
    }
}
