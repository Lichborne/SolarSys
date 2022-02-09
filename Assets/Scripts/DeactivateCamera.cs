using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateCamera : MonoBehaviour{

    public MonoBehaviour DisableScript;
    public void deactivateCamera()
    {
        DisableScript.enabled = !DisableScript.enabled;
    }
}
