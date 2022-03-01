using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateCamera : MonoBehaviour{

    public MonoBehaviour DisableScript;

    void Start()
    {
        DisableScript.enabled = false;
    }
    
    public void activateCamera()
    {
        DisableScript.enabled = true;
    }

    public void toggleCamera()
    {
        DisableScript.enabled = !DisableScript.enabled;
    }
}
