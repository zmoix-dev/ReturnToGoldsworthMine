using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class ActionScheduler : MonoBehaviour
    {
        IAction currentAction;
        public void StartAction(IAction action) {
            if (currentAction != null && currentAction != action) {
                currentAction.Stop();
            }
            currentAction = action;
        }

        public void StopCurrentAction() {
            if (currentAction != null) {
                currentAction.Stop();
                currentAction = null;
            }
        }
    }
}