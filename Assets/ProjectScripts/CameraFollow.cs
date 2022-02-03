using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // in our case this will always be camera.Main, but it is good practice to 
    // have a data member and initialize /w start() for future flexibility
    public Camera cameraToLookAt;
   
    void Start() 
    {
        // for now, this is always main; this can be removed and cameraToLookAt changed
        // from Unity direclty, or changed here if we decide to go another way
        cameraToLookAt = Camera.main;
    }
 
    void Update() 
    {
        // transform to always look at the camera
        transform.rotation = Quaternion.LookRotation(-cameraToLookAt.transform.forward, cameraToLookAt.transform.up);
        transform.Rotate(0, 180, 0);
    }
}