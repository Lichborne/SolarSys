using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterCamera : MonoBehaviour
{
    public Camera cameraToRecenter = Camera.main;
   
    void Start() 
    {
        recenter();
    }

    void recenter() 
    {
        cameraToRecenter = Camera.main;
        transform.rotation = new Quaternion(0,0,0,0);
        transform.position = new Vector3(0,0,0);
    }

    
}
