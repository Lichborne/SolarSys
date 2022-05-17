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
            // try
            // {
                GameObject currentlySelectedObject = findCurrentlySelectedPlanetorEdge();
                if (currentlySelectedObject.tag == "Node")
                {
                    GraphNode attachedNode = currentlySelectedObject.GetComponent<FrontEndNode>().getDatabaseNode();
                    Camera.main.GetComponent<CoroutineRunner>().RunCoroutine(attachedNode.UpdateTitleCo(titleEntry));
                    // StartCoroutine(attachedNode.UpdateTitleCo(titleEntry));
                    
                    //update UI
                    // Canvas canvas = currentlySelectedObject.GetComponent<Canvas>();
                    // Debug.Log(canvas.GetComponent<TMP_InputField>().text);
                    ChangeText.ChangeInputFieldText(currentlySelectedObject, titleEntry);

                }
                else if (currentlySelectedObject.tag == "Edge")
                {
                    GraphEdge attachedEdge = currentlySelectedObject.GetComponent<FrontEndEdge>()._databaseEdge;
                    Camera.main.GetComponent<CoroutineRunner>().RunCoroutine(attachedEdge.UpdateTitleCo(titleEntry));
                    // StartCoroutine(attachedEdge.UpdateTitleCo(titleEntry));

                    ChangeText.ChangeInputFieldText(currentlySelectedObject.GetComponent<FrontEndEdge>()._textObject, titleEntry);
                    // currentlySelectedObject.GetComponent<FrontEndEdge>()._textObject.text=titleEntry;
                }

                // Update UI

            // }
            // catch
            // {
            //     Debug.Log("Please select a planet first");
            // }
        } 
        Camera.main.GetComponent<CameraReadOnly>().readOnly = false;
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
        Debug.Log("Title" + currentlySelectedObject);
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