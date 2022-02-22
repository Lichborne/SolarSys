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
            GameObject currentlySelectedPlanet = findCurrentlySelectedPlanet();
            try
            {
                GraphNode attachedNode = currentlySelectedPlanet.GetComponent<FrontEndNode>().getDatabaseNode();
                StartCoroutine(attachedNode.UpdateTitleCo(titleEntry));
                // inputField.GetComponent<TMP_InputField>().text = ""; // reset text field
            }
            catch(InvalidOperationException)
            {
                Debug.Log("Please select a planet first");
            }
        } 
    }

    private GameObject findCurrentlySelectedPlanet()
    {
        GameObject currentlySelectedGameObject = Camera.main.GetComponent<Click>().selectedObject;
        return currentlySelectedGameObject;
    }

    public void displayTitle()
    {// To be called when "Edit Text is called", will update description to 
        GameObject currentlySelectedPlanet = findCurrentlySelectedPlanet();
        GraphNode attachedNode = currentlySelectedPlanet.GetComponent<FrontEndNode>().getDatabaseNode();
        inputField.GetComponent<TMP_InputField>().text = attachedNode.Title;
    }
}