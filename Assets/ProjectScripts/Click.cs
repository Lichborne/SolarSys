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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit rayHit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, clickablesLayer))
            {   
                ClickOn clickOnScript = rayHit.collider.GetComponent<ClickOn>();
                if (Input.GetKey("left ctrl"))
                {
                    if (clickOnScript.currentlySelected == false)
                    {
                        selectedObjects.Add(rayHit.collider.gameObject);
                        clickOnScript.currentlySelected = true;
                        clickOnScript.ClickMe();
                    }
                    else
                    {
                        selectedObjects.Remove(rayHit.collider.gameObject);
                        clickOnScript.currentlySelected = false;
                        clickOnScript.ClickMe();
                    }
                }
                else
                {
                    if (selectedObjects.Count > 0)
                    {
                        foreach (GameObject obj in selectedObjects) 
                        {
                            obj.GetComponent<ClickOn>().currentlySelected = false;
                            obj.GetComponent<ClickOn>().ClickMe();
                        }

                        selectedObjects.Clear();
                    }

                    selectedObjects.Add(rayHit.collider.gameObject);
                    clickOnScript.currentlySelected = true;
                    clickOnScript.ClickMe();
                }
                
            }

            else 
            {
                if (!EventSystem.current.IsPointerOverGameObject()) 
                {
                    if (selectedObjects.Count > 0)
                    {
                        foreach (GameObject obj in selectedObjects) 
                        {
                            obj.GetComponent<ClickOn>().currentlySelected = false;
                            obj.GetComponent<ClickOn>().ClickMe();
                        }

                        selectedObjects.Clear();
                    }
                }
                
            }

            if (selectedObjects.Count == 1)
            {
                selectedObject = selectedObjects[0];
            }

            if (selectedObjects.Count > 1) 
            {
                UIPanel.SetActive(false);
                UIPanelPath.SetActive(false);
                UIPanelMultiple.SetActive(true);
            }
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

    public void ShowPath()
    {   
        isShowingPath = true;
        UIPanel.SetActive(false);
        UIPanelMultiple.SetActive(false);
        UIPanelPath.SetActive(true);

        if (selectedObjects.Count > 0)
        {

            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
            {   
                hiddenNodes.Add(obj);
                obj.GetComponent<ClickOn>().currentlyHidden = true;
                obj.GetComponent<ClickOn>().HideUnhideMe();
            }

            foreach (GameObject obj in selectedObjects) 
            {   
                obj.GetComponent<ClickOn>().currentlySelected = false;
                obj.GetComponent<ClickOn>().ClickMe();
                hiddenNodes.Remove(obj);
                obj.GetComponent<ClickOn>().currentlyHidden = false;
                obj.GetComponent<ClickOn>().HideUnhideMe();
            }

            selectedObjects.Clear();

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

    public void HidePath()
    {   
        isShowingPath = false;
        UIPanelPath.SetActive(false);
        UIPanelMultiple.SetActive(false);
        UIPanel.SetActive(true);

        foreach (GameObject obj in selectedObjects) 
        {   
            obj.GetComponent<ClickOn>().currentlySelected = false;
            obj.GetComponent<ClickOn>().ClickMe();
        }

        selectedObjects.Clear();

        foreach (GameObject obj in hiddenNodes)
        {
            obj.GetComponent<ClickOn>().currentlyHidden = false;
            obj.GetComponent<ClickOn>().HideUnhideMe(); 
        }

        foreach (GameObject obj in hiddenEdges)
        {  
            obj.SetActive(true);
        }

    }
}
