using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Backend;

public class DisplayNodeDesciption : MonoBehaviour
{
    // This script should be attached to the description and will allow the user to see the descriptions
    // of a node called from the database

    public GameObject titleField;

    public GameObject descriptionField;

    private TMP_Text m_TitleText;

    private TMP_Text m_DescriptionText;

    public void displayDescription()
    {
        // try
        // {
            GameObject currentlySelectedObject = findCurrentlySelectedPlanetorEdge();
            m_TitleText = titleField.GetComponent<TMP_Text>();
            m_DescriptionText = descriptionField.GetComponent<TMP_Text>();
            if (currentlySelectedObject.tag == "Node")
            {
                GraphNode attachedNode = currentlySelectedObject.GetComponent<FrontEndNode>().getDatabaseNode();    
                m_TitleText.text = attachedNode.Title;
                m_DescriptionText.text = attachedNode.Description;
            }
            else if (currentlySelectedObject.tag == "Edge")
            {
                GraphEdge attachedEdge= currentlySelectedObject.GetComponent<FrontEndEdge>()._databaseEdge;    
                m_TitleText.text = attachedEdge.Title;
                m_DescriptionText.text = attachedEdge.Description;
            }
        // }             
        // catch
        // {
        //     Debug.Log("Please select a planet or an edge first");
        // }
    }
    
    private GameObject findCurrentlySelectedPlanetorEdge()
    {

         // If no planet was found then they must have chosen an edge
        GameObject currentlySelectedObject = Camera.main.GetComponent<Click>().selectedObject;
        if (Camera.main.GetComponent<Click>().selectedObjects.Count == 0) 
        {
            currentlySelectedObject = Camera.main.GetComponent<Click>().selectedEdge;
        }
        return currentlySelectedObject;
    }

}
