using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core {
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] Transform followTarget;
        Vector3 cameraOffset;
        void Start() {
            cameraOffset = new Vector3(transform.position.x - followTarget.transform.position.x, 
                transform.position.y - followTarget.transform.position.y, 
                transform.position.z - followTarget.transform.position.z
                );
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = followTarget.position + cameraOffset;
        }
    }
}