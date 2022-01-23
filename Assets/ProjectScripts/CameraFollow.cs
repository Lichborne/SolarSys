using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Camera cameraToLookAt;
   
 void Start() 
 {
     //transform.Rotate( 180,0,0 );
     cameraToLookAt = Camera.main;
 }
 
 void Update() 
 {
     {
         transform.rotation = Quaternion.LookRotation(-cameraToLookAt.transform.forward, cameraToLookAt.transform.up);
         transform.Rotate( 0,180,0 );

         //transform.forward = new Vector3(Camera.main.transform.forward.x, //transform.forward.y, Camera.main.transform.forward.z);
     }
 }
}