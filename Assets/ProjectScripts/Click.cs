using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour
{
    [SerializeField]
    private LayerMask clickableNodeLayer;
    [SerializeField]
    private LayerMask clickableEdgeLayer;
    [HideInInspector]
    public GameObject selectedObject;
    [HideInInspector]
    public GameObject selectedEdge;
    public List<GameObject> selectedObjects;
    private List<GameObject> hiddenNodes;
    private List<GameObject> hiddenEdges;
    public List<GameObject> shownNodes;
    public GameObject UIPanel;
    public GameObject UIPanelPath;
    public GameObject UIPanelMultiple;
    public GameObject UIPanelEdge;
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

            //If clicked on an edge
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, clickableEdgeLayer))
            {
                selectedEdge = rayHit.collider.gameObject;
            }
             //if clicked on an empty space
            else 
            {
                //If pointer is not over (clicking on UI)
                if (!EventSystem.current.IsPointerOverGameObject()) 
                {
                   selectedEdge = null;  
                }
            }

            //If clicked on something on a clickableNodeLayer (planet), below if else all related to planets
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, clickableNodeLayer))
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

           
            //If is showing path
            if(isShowingPath)
            {
                UIPanelPath.SetActive(true);
                UIPanel.SetActive(false);
                UIPanelMultiple.SetActive(false);
                UIPanelEdge.SetActive(false);

            }
            //If is not showing path
            else{
                //if there is an edge selected
                if (selectedEdge != null) 
                {
                    UIPanel.SetActive(false);
                    UIPanelMultiple.SetActive(false);
                    UIPanelPath.SetActive(false);
                    UIPanelEdge.SetActive(true);
                }
                else
                {
                    //if selectedObjects is empty when not in path mode
                    if (selectedObjects.Count == 0)
                    {
                        UIPanel.SetActive(false);
                        UIPanelMultiple.SetActive(false);
                        UIPanelPath.SetActive(false);
                        UIPanelEdge.SetActive(false);
                    }

                    //if selectedObjects only countatins one selected node
                    if (selectedObjects.Count == 1)
                    {
                        selectedObject = selectedObjects[0];
                        UIPanel.SetActive(true);
                        UIPanelMultiple.SetActive(false);
                        UIPanelPath.SetActive(false);
                        UIPanelEdge.SetActive(false);
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
                        UIPanelEdge.SetActive(false);
                    }
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
        UIPanelEdge.SetActive(false);

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
                shownNodes.Add(obj);
                obj.GetComponent<ClickOn>().currentlyHidden = false;
                obj.GetComponent<ClickOn>().HideUnhideMe();
            }
            //clear selected objects
            selectedObjects.Clear();

            //hide edges which are not between nodes
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Edge")) 
            {
                if (obj.name != "EdgePrefab" && (
                    obj.GetComponent<FrontEndEdge>()._parent.activeSelf == false ||
                    obj.GetComponent<FrontEndEdge>()._child.activeSelf == false))
                {
                    hiddenEdges.Add(obj);
                    obj.SetActive(false);
                    obj.GetComponent<FrontEndEdge>()._textObject.SetActive(false);
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
        UIPanelEdge.SetActive(false);

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
            obj.GetComponent<FrontEndEdge>()._textObject.SetActive(true);

        }
        shownNodes.Clear();
    }
}
