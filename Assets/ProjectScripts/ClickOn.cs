using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;


public class ClickOn : MonoBehaviour
{   
    [SerializeField]
    private Material unselected;
    [SerializeField]
    private Material selected;

    [HideInInspector]
    public bool currentlySelected = false;
    [HideInInspector]
    public bool currentlyHidden = false;

    private Backend.GraphProject graph = new Backend.Graph();

    private MeshRenderer myRend;
    // Start is called before the first frame update
    void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        ClickMe();

    }

    //Function that gets called by Click class in order to change planet materail
    public void ClickMe() 
    {
        if (currentlySelected == false) 
        {
            myRend.material = unselected;
        }
        else
        {
            getNodeOfSelectedPlanet();
            myRend.material = selected;
        }

    }

    //Function that gets called by Click class in order to hide/unhide objects
    public void HideUnhideMe() 
    {
        if (currentlyHidden == false) 
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    //Josh's function
    public void getNodeOfSelectedPlanet()
    {
        FrontEndNode node = GetComponent<FrontEndNode>();
        Debug.Log(node.getDatabaseNode().Text);
    }
   
}
