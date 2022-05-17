using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Backend;
public class ModifyPlanetDescription : MonoBehaviour
{
    // This script will allow the user to change the description associated with a planet gameObject by making calls to the 
    // GraphProject class
    [HideInInspector]
    public string descriptionEntry;
    public GameObject inputField;

    public void updateInput()
    {
        // Get entry from description box and update corresponding selected node
        descriptionEntry = inputField.GetComponent<TMP_InputField>().text;
        if (descriptionEntry != "") // Users cannot input null entries
        {
            // try
            // {
            GameObject currentlySelectedObject = findCurrentlySelectedPlanetorEdge();
            if (currentlySelectedObject.tag == "Node")
            {
                GraphNode attachedNode = currentlySelectedObject.GetComponent<FrontEndNode>().getDatabaseNode();
                StartCoroutine(attachedNode.UpdateDescriptionCo(descriptionEntry));
            }
            else if (currentlySelectedObject.tag == "Edge")
            {
                GraphEdge attachedEdge = currentlySelectedObject.GetComponent<FrontEndEdge>()._databaseEdge;
                StartCoroutine(attachedEdge.UpdateDescriptionCo(descriptionEntry));
            }


            //Update the frontend text
            // inputField.GetComponent<TMP_InputField>().text = ""; // reset text field
            // }
            // catch
            // {
            //     Debug.Log("Please select a planet or an edge first");
            // }
        } 
        Camera.main.GetComponent<CameraReadOnly>().readOnly = false;
    }

    private GameObject findCurrentlySelectedPlanetorEdge()
    {
        GameObject currentlySelectedObject = Camera.main.GetComponent<Click>().selectedObject;
        Debug.Log(currentlySelectedObject);
        if (!currentlySelectedObject) // If no planet was found then they must have chosen an edge
        {
            currentlySelectedObject = Camera.main.GetComponent<Click>().selectedEdge;
        }
        // Debug.Log("Currently selected object = " + currentlySelectedObject);
        return currentlySelectedObject;
    }

    public void displayDescription()
    {// To be called when "Edit Text is called", will update description to 
        GameObject currentlySelectedObject = findCurrentlySelectedPlanetorEdge();
        if (currentlySelectedObject.tag == "Node")
        {
            GraphNode attachedNode = currentlySelectedObject.GetComponent<FrontEndNode>().getDatabaseNode();
            inputField.GetComponent<TMP_InputField>().text = attachedNode.Description;
        }
        else if (currentlySelectedObject.tag == "Edge")
        {
            GraphEdge attachedEdge = currentlySelectedObject.GetComponent<FrontEndEdge>()._databaseEdge;
            inputField.GetComponent<TMP_InputField>().text = attachedEdge.Description;
        }
    }
}
