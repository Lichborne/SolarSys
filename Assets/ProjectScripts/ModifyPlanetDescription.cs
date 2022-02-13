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
            GameObject currentlySelectedPlanet = findCurrentlySelectedPlanet();
            try
            {
                GraphNode attachedNode = currentlySelectedPlanet.GetComponent<FrontEndNode>().getDatabaseNode();
                attachedNode.UpdateDescription(descriptionEntry);
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

    public void displayDescription()
    {// To be called when "Edit Text is called", will update description to 
        GameObject currentlySelectedPlanet = findCurrentlySelectedPlanet();
        GraphNode attachedNode = currentlySelectedPlanet.GetComponent<FrontEndNode>().getDatabaseNode();
        inputField.GetComponent<TMP_InputField>().text = attachedNode.Description;
    }
}
