using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats {
    public interface IModifierProvider {
        IEnumerable<float> GetAdditiveModifiers(StatsType stat);
        IEnumerable<float> GetPercentageModifiers(StatsType stat);
    }
}
