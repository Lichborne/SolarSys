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
        GameObject currentlySelectedPlanet = findCurrentlySelectedPlanet();
        GraphNode attachedNode = currentlySelectedPlanet.GetComponent<FrontEndNode>().getDatabaseNode();

        m_TitleText = titleField.GetComponent<TMP_Text>();
        m_DescriptionText = descriptionField.GetComponent<TMP_Text>();

        m_TitleText.text = attachedNode.Title;
        m_DescriptionText.text = attachedNode.Description;
    }
    private GameObject findCurrentlySelectedPlanet()
    {
        GameObject currentlySelectedGameObject = Camera.main.GetComponent<Click>().selectedObject;
        return currentlySelectedGameObject;
    }
}
