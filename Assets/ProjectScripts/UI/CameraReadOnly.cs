using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraReadOnly : MonoBehaviour
{

    public GameObject buttonEditText1; //UI Panel for nodes
    public GameObject player;
    public GameObject buttonEditText2; // UI panel for edges
    public GameObject sphere;
    // Start is called before the first frame update
    [HideInInspector]
    public bool readOnly = false;
   

    public void makeReadOnly()
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
            Debug.Log("Turning on functionality");
            triggerFunctionality(true);
        }
    }

    private void triggerFunctionality(bool trigger)
    {
        Debug.Log("Turning functionality to " + trigger);

        sphere.GetComponent<NodeOperations>().enabled = trigger;
        Camera.main.GetComponent<AddNodeManual>().enabled = trigger;
        player.GetComponent<AddEdge>().enabled = trigger;
        buttonEditText1.SetActive(trigger);
        buttonEditText2.SetActive(trigger);
    }

}
