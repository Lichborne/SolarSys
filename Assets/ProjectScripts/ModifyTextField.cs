using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Backend;
public class ModifyTextField : MonoBehaviour
{
    // This script will allow the user to change the text associated with a planet gameObject by making calls to the 
    // GraphProject class
    [HideInInspector]
    public string textEntry;
    public GameObject inputField;
    // Start is called before the first frame update

    public void storeInput()
    {
        textEntry = inputField.GetComponent<TMPro.TextMeshProUGUI>().text;
        GameObject currentlySelectedGameObject = findCurrentlySelectedNode();
        try{
            Debug.Log(currentlySelectedGameObject.GetComponent<FrontEndNode>().databaseNode.Text);
        }
        catch(InvalidOperationException)
        {
            throw new Exception("Error no node is currently selected");  
        }
    }

    private GameObject findCurrentlySelectedNode()
    {

            GameObject currentlySelectedGameObject = Camera.main.GetComponent<Click>().selectedObject;
            return currentlySelectedGameObject;
    }

}
