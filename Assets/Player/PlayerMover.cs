using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMover : MonoBehaviour
{
    Ray lastRay;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) {
            MoveToClick();
        }
    }

    private void MoveToClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit) {
            GetComponent<NavMeshAgent>().destination = hit.point;
        }
        
    }
}
