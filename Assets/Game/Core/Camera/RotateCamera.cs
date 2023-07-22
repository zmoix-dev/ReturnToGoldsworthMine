using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.Camera {
    public class RotateCamera : MonoBehaviour
    {
        [SerializeField] float rotationSpeed = 1f;
        [SerializeField] GameObject player;
        void Update()
        {
            if (player != null) {
                if (Input.GetKey(KeyCode.Q)) {
                    transform.RotateAround(player.transform.position, Vector3.up, 90 * (rotationSpeed * Time.deltaTime));
                } 
                if (Input.GetKey(KeyCode.E)) {
                    transform.RotateAround(player.transform.position, Vector3.up, -90 * (rotationSpeed * Time.deltaTime));
                }
            }
            
        }
    }
}