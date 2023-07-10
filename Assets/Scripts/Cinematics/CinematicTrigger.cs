using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicTrigger : MonoBehaviour
    {
        bool hasPlayed = false;
        private void OnTriggerEnter(Collider other) {
            if (!hasPlayed && other.tag.Equals(UnitType.GetType(UnitType.Type.PLAYER))) {
                GetComponent<PlayableDirector>().Play();
                hasPlayed = true;
            }
        }
    }
}

