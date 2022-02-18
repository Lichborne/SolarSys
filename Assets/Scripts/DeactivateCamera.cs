using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateCamera : MonoBehaviour{

    public MonoBehaviour DisableScript;
    public void toggleCamera()
    {
        DisableScript.enabled = !DisableScript.enabled;
    }    
    
    public void activateCamera()
    {
        DisableScript.enabled = true;
    }
}
