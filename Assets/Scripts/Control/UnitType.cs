using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control {
    public class UnitType
    {
        private static string[] unitTags = {"Player", "Ally", "Enemy"};

        public static string GetUnitType(UnitTypes index) {
            return unitTags[(int)index];
        }
        public enum UnitTypes {
            PLAYER = 0,
            ALLY = 1,
            ENEMY = 2
        }
        
    }
}
