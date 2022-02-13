using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour
{
    [SerializeField]
    private LayerMask clickablesLayer;
    [HideInInspector]
    public GameObject selectedObject;
    private List<GameObject> selectedObjects;
    private List<GameObject> hiddenNodes;
    private List<GameObject> hiddenEdges;
    public GameObject UIPanel;
    public GameObject UIPanelPath;
    public GameObject UIPanelMultiple;
    private bool isShowingPath = false;


    void Start()
    {
        selectedObjects = new List<GameObject>();
        hiddenNodes = new List<GameObject>();
        hiddenEdges = new List<GameObject>();
        UIPanel.SetActive(false);
        UIPanelMultiple.SetActive(false);
        UIPanelPath.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {   
        //On left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit rayHit;

            //If clicked on something on a clickablesLayer
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, clickablesLayer))
            {   
                ClickOn clickOnScript = rayHit.collider.GetComponent<ClickOn>();
                if (Input.GetKey("left ctrl"))
                {   
                    //if clicked object is not selected, add object to currentlySelected list and update it
                    if (clickOnScript.currentlySelected == false)
                    {
                        selectedObjects.Add(rayHit.collider.gameObject);
                        clickOnScript.currentlySelected = true;
                        clickOnScript.ClickMe();
                    }
                    //if clicked object is selected, remove object from currentlySelected list and update it
                    else
                    {   
                        selectedObjects.Remove(rayHit.collider.gameObject);
                        clickOnScript.currentlySelected = false;
                        clickOnScript.ClickMe();
                    }
                }
                //if ctrl key is not pressed when mouse button is down
                else
                {
                    //wipe all objects from the currentlySelected list and update them
                    if (selectedObjects.Count > 0)
                    {
                        foreach (GameObject obj in selectedObjects) 
                        {
                            obj.GetComponent<ClickOn>().currentlySelected = false;
                            obj.GetComponent<ClickOn>().ClickMe();
                        }

                        selectedObjects.Clear();
                    }

                    //selected object that is clicked on and add to list
                    selectedObjects.Add(rayHit.collider.gameObject);
                    clickOnScript.currentlySelected = true;
                    clickOnScript.ClickMe();
                }
                
            }

            //if clicked on an empty space
            else 
            {
                //If pointer is not over (clicking on UI)
                if (!EventSystem.current.IsPointerOverGameObject()) 
                {
                    //If there is more than 0 objects on the selectedObjects list deselect them
                    if (selectedObjects.Count > 0)
                    {
                        foreach (GameObject obj in selectedObjects) 
                        {
                            obj.GetComponent<ClickOn>().currentlySelected = false;
                            obj.GetComponent<ClickOn>().ClickMe();
                        }

                        //clear list
                        selectedObjects.Clear();
                    }
                }
                
            }

            //if selectedObjects only countatins one selected node, might be useful to Josh
            if (selectedObjects.Count == 1)
            {
                selectedObject = selectedObjects[0];
                UIPanel.SetActive(true);
                UIPanelMultiple.SetActive(false);
                UIPanelPath.SetActive(false);
            }
            else {
                selectedObject = null;
            }

            //if more than one selected objects - change UI view
            if (selectedObjects.Count > 1) 
            {
                UIPanel.SetActive(false);
                UIPanelPath.SetActive(false);
                UIPanelMultiple.SetActive(true);
            }
            //if less or equal to one object selected - change UI view
            else
            {
                UIPanelMultiple.SetActive(false);
                if(isShowingPath)
                {
                    UIPanelPath.SetActive(true);
                    UIPanel.SetActive(false);
                }
                else{
                    UIPanel.SetActive(true);
                    UIPanelPath.SetActive(false);
                }
                
            }
        }

    }

    //function to hide all nodes apart from those selected
    public void ShowPath()
    {   
        isShowingPath = true;
        UIPanel.SetActive(false);
        UIPanelMultiple.SetActive(false);
        UIPanelPath.SetActive(true);

        if (selectedObjects.Count > 0)
        {
            //hide all nodes first
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
            {   
                hiddenNodes.Add(obj);
                obj.GetComponent<ClickOn>().currentlyHidden = true;
                obj.GetComponent<ClickOn>().HideUnhideMe();
            }
            //unhide selected nodes
            foreach (GameObject obj in selectedObjects) 
            {   
                obj.GetComponent<ClickOn>().currentlySelected = false;
                obj.GetComponent<ClickOn>().ClickMe();
                hiddenNodes.Remove(obj);
                obj.GetComponent<ClickOn>().currentlyHidden = false;
                obj.GetComponent<ClickOn>().HideUnhideMe();
            }
            //clear selected objects
            selectedObjects.Clear();

            //hide edges which are not between nodes
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Edge")) 
            {
                if (obj.GetComponent<StoreParentChild>().parent.activeSelf == false || obj.GetComponent<StoreParentChild>().child.activeSelf == false)
                {
                    hiddenEdges.Add(obj);
                    obj.SetActive(false);
                }
                
            }


        }
    }

    //function to restore all hidden nodes and edges
    public void HidePath()
    {   
        isShowingPath = false;
        UIPanelPath.SetActive(false);
        UIPanelMultiple.SetActive(false);
        UIPanel.SetActive(false);

        //deselect everything
        foreach (GameObject obj in selectedObjects) 
        {   
            obj.GetComponent<ClickOn>().currentlySelected = false;
            obj.GetComponent<ClickOn>().ClickMe();
        }

        selectedObjects.Clear();
        //unhide all nodes
        foreach (GameObject obj in hiddenNodes)
        {
            obj.GetComponent<ClickOn>().currentlyHidden = false;
            obj.GetComponent<ClickOn>().HideUnhideMe(); 
        }
        //unhide all edges
        foreach (GameObject obj in hiddenEdges)
        {  
            obj.SetActive(true);
        }

    }
}
