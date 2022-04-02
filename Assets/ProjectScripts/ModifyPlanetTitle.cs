using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Backend;
public class ModifyPlanetTitle: MonoBehaviour
{
    // This script will allow the user to change the title associated with a planet gameObject by making calls to the 
    // GraphProject class
    [HideInInspector]
    public string titleEntry;
    public GameObject inputField;

    public void updateInput()
    {
        // Get entry from description box and update corresponding selected node
        titleEntry = inputField.GetComponent<TMP_InputField>().text;
        if (titleEntry != "") // Users cannot input null entries
        {
            try
            {
                GameObject currentlySelectedObject = findCurrentlySelectedPlanetorEdge();
                if (currentlySelectedObject.tag == "Node")
                {
                    GraphNode attachedNode = currentlySelectedObject.GetComponent<FrontEndNode>().getDatabaseNode();
                    StartCoroutine(attachedNode.UpdateTitleCo(titleEntry));
                }
                else if (currentlySelectedObject.tag == "Edge")
                {
                    GraphEdge attachedEdge = currentlySelectedObject.GetComponent<FrontEndEdge>()._databaseEdge;
                    StartCoroutine(attachedEdge.UpdateTitleCo(titleEntry));
                }    
            }
            catch
            {
                Debug.Log("Please select a planet first");
            }
        } 
    }

    private GameObject findCurrentlySelectedPlanetorEdge()
    {
        GameObject currentlySelectedObject = Camera.main.GetComponent<Click>().selectedObject;
        if (!currentlySelectedObject) // If no planet was found then they must have chosen an edge
        {
            currentlySelectedObject = Camera.main.GetComponent<Click>().selectedEdge;
        }
        // Debug.Log("Currently selected object = " + currentlySelectedObject);
        return currentlySelectedObject;
    }
    public void displayTitle()
    {// To be called when "Edit Text is called", will update description to 
        GameObject currentlySelectedObject = findCurrentlySelectedPlanetorEdge();
        if (currentlySelectedObject.tag == "Node")
        {
            GraphNode attachedNode = currentlySelectedObject.GetComponent<FrontEndNode>().getDatabaseNode();
            inputField.GetComponent<TMP_InputField>().text = attachedNode.Title;
        }
        else if (currentlySelectedObject.tag == "Edge")
        {
            GraphEdge attachedEdge = currentlySelectedObject.GetComponent<FrontEndEdge>()._databaseEdge;
            inputField.GetComponent<TMP_InputField>().text = attachedEdge.Title;
        }
    }
}