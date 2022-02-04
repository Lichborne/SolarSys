using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera cameraToLookAt;
   
    void Start() 
    {
        cameraToLookAt = Camera.main;
    }
 
    void Update() 
    {
        transform.rotation = Quaternion.LookRotation(-cameraToLookAt.transform.forward, cameraToLookAt.transform.up);
        transform.Rotate(0, 180, 0);
    }
}