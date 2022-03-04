using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterCamera : MonoBehaviour
{
    
    private Camera cameraToRecenter = Camera.main;

    //void OnMouseOver() {
        //Debug.Log("touched");
        //if (Input.GetMouseButtonDown(0)) {
            //recenter();
        //}
    //}

    public void recenter() 
    {
        GameObject player = GameObject.FindGameObjectsWithTag("Player")[0];
        cameraToRecenter = Camera.main;
        cameraToRecenter.transform.rotation = new Quaternion(0,0,0,0);
        cameraToRecenter.transform.position = new Vector3(0,0,0);
        player.transform.position = new Vector3(0,0,0);
        Debug.Log("Called");
    }

    
}
