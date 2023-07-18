using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] GameObject parent;

    public void Destroy(){ 
        Destroy(parent);
    }
}
