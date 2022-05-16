using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReadOnly : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]
    public bool readOnly = false;

    void makeReadOnly()
    {
        readOnly = true;
    }

    void Update(){
        if (readOnly)
        {
            triggerFunctionality(false);
        }
        else
        {
            triggerFunctionality(true);
        }
    }

    private void triggerFunctionality(bool trigger)
    {
        if (GameObject.Find("Sphere") != null)
        {
            GameObject.Find("Sphere").GetComponent<Drag>().enabled = trigger;
        }
        if (GameObject.Find("Button-EditText") != null)
        {
            GameObject.Find("Button-EditText").SetActive(trigger);
        }
        if (GameObject.Find("Button-NewNode") != null)
        {
            GameObject.Find("Button-NewNode").SetActive(trigger);
        }
    }

}
