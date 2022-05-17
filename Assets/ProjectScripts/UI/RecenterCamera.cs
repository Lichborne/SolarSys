using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterCamera : MonoBehaviour
{
    
    //private Camera cameraToRecenter = Camera.main;

    //void OnMouseOver() {
        //Debug.Log("touched");
        //if (Input.GetMouseButtonDown(0)) {
            //recenter();
        //}
    //}

    public void recenter() 
    {
        //Camera camera = gameObject.GetComponent<Camera>();
        Camera cameraToRecenter = Camera.main;

        GameObject dummy = new GameObject();

        // Work with the transform's variables
        dummy.transform.position = new Vector3(0, 0, 0);
        dummy.transform.rotation = Quaternion.Euler(180, 0, 180);
        dummy.transform.localScale = new Vector3(1, 1, 1);

        cameraToRecenter.GetComponent<UnityTemplateProjects.SimpleCameraController>().m_TargetCameraState.SetFromTransform(dummy.transform);
        gameObject.transform.position = new Vector3(0,0,0);

        Destroy(dummy);

        // Debug.Log("Called");
    }

    
}
