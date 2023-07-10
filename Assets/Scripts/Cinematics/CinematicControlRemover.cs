using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicControlRemover : MonoBehaviour
    {
        public void Start() {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }
        void DisableControl(PlayableDirector director) {
            GameObject player = GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER));
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<ActionScheduler>().StopCurrentAction();
        }

        void EnableControl(PlayableDirector director) {
            GameObject.FindWithTag(UnitType.GetType(UnitType.Type.PLAYER)).GetComponent<PlayerController>().enabled = true;
        }
    }
}