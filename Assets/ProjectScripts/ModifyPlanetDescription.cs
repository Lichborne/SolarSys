using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Backend;
public class ModifyPlanetDescription : MonoBehaviour
{
    // This script will allow the user to change the text associated with a planet gameObject by making calls to the 
    // GraphProject class
    [HideInInspector]
    public string descriptionEntry;
    public GameObject inputField;
    // Start is called before the first frame update

    public void updateInput()
    {
        // Get entry from description box and update corresponding selected node
        descriptionEntry = inputField.GetComponent<TMPro.TextMeshProUGUI>().text;
        GameObject currentlySelectedPlanet = findCurrentlySelectedPlanet();
        GraphNode attachedNode = currentlySelectedPlanet.GetComponent<FrontEndNode>().getAttachedNode();
        attachedNode.UpdateDescription(descriptionEntry);
    }

    private GameObject findCurrentlySelectedPlanet()
    {
        GameObject currentlySelectedGameObject = Camera.main.GetComponent<Click>().selectedObject;
        return currentlySelectedGameObject;
    }
}
