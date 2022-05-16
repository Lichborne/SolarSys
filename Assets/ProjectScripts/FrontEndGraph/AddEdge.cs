using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;
using UnityEngine.EventSystems;
// This class handles adding edges. We can only inherit from MonoBehaviour, so although the data members are similar to other classes,
// we cannot have an abstract class. I would also like to avoid touching MonoBehaviour; it's a delicate beast, and central to 
// everything we do in Unity.
public class AddEdge : MonoBehaviour
{
    // The parent node the edge originates from, named differently because it si conceptually 
    // representative a different kind of relationship than inside the class
    private GameObject _fromNode = null;

    private GameObject _toNode = null;                  // The child node the edge points to; same as above.
    
    public GameObject _edgePrefab = null;               // Straight edge prefab
    
    public GameObject _curvedPrefab = null;             // Curved edge prefab
    
    public GameObject _selfReferencePreFab = null;      // Self reference prefab

    // needed because we have to freeze the camera when adding an edge, otherwise controls are very uncomfortable
    public MonoBehaviour cameraController;
    
    // This is the separate text for edges; cannot be a part of the prefabs due to universal scaling being uncircumventable from inside game object
    public GameObject _textObject = null;

    // Behaviour is slightly different when we have to be placing a curved edge
    private bool isCurvedEdge { get; set; } = false;

    // True when we are in the (potential) process of adding an edge
    private bool adding { get; set; } = false;

    // Panel States to check                  
    private GameObject[] panels;

    void Start() 
    {
        panels = GameObject.FindGameObjectsWithTag("Panel");
    }

    // Update is called once per frame
    void Update()
    {
        // depending on the order things are created we may not yet have the panels before start is called,
        // so we do have to poll for this
        if (panels.Length == 0) panels = GameObject.FindGameObjectsWithTag("Panel");
        
        // don't run if any of the panels are active
        foreach (GameObject p in panels) if (p.activeSelf) return; 

        // We use raycasts and update, putting this whole behaviour separately from functionality in Drag, 
        // becuase we have to drag the click, the possible hold, and release, and wehther that release hits another node object;
        // while  all of this would be possible in that context, since we need to operate between frames, it is cleaner
        // this way, and Drag is already, if anything, a little cluttered.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        // If there is a click, and we are hitting a node -> we are adding an edge
        if ((Input.GetKeyDown("3") || Input.GetMouseButtonDown(1)) && Physics.Raycast(ray, out hitInfo, Mathf.Infinity) && hitInfo.transform.tag == "Node")
        {   
            // when we are adding an edge, we do not want the mouse to move the camera.
            cameraController.enabled = false;
            _fromNode = hitInfo.collider.gameObject;
            adding = true;
            Debug.Log("ADDING");

        }

        // If there is a button release, if we weren't adding we do nothing, if we are adding but we are not hitting
        //  node by raycast, we do nothing and reset adding, and if we do hit a node, then, finally, we add an edge
        if((Input.GetKeyUp("3") || Input.GetMouseButtonUp(1)) ) 
        {
            if (!adding) 
            {
                Debug.Log("NOT ADDING");
                return;
            }

            //no matter what happens, we let go of the camera as early as possible.
            cameraController.enabled = true;
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo2;

            if (Physics.Raycast(ray2, out hitInfo2, Mathf.Infinity) && hitInfo2.transform.tag == "Node") 
            {
                _toNode = hitInfo2.collider.gameObject;

                GameObject rightPrefab = _edgePrefab;

                isCurvedEdge = false;

                float rotation = 0;

                // we determine what kind of an edge we have to add, and act accordingly
                if(_toNode == _fromNode) 
                {
                    rightPrefab = _selfReferencePreFab;
                    rotation = _fromNode.GetComponent<FrontEndNode>().changeEdge(_toNode, _selfReferencePreFab);
                } 
                else if (_fromNode.GetComponent<FrontEndNode>().from.Contains(_toNode) || _toNode.GetComponent<FrontEndNode>().from.Contains(_fromNode)) 
                {
                    rightPrefab = _curvedPrefab;
                    rotation = _fromNode.GetComponent<FrontEndNode>().changeEdge(_toNode, _curvedPrefab);
                    isCurvedEdge = true;
                } 

                //if we get time, this should be turned into a function as it recurrs
                GameObject edgeObject = Instantiate(rightPrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
                GraphEdge databaseEdge = new GraphEdge("New Edge", ". . .", _fromNode.GetComponent<FrontEndNode>()._databaseNode, _toNode.GetComponent<FrontEndNode>()._databaseNode);
                
                StartCoroutine(databaseEdge.CreateInDatabaseCo());
                
                GameObject textObject = Instantiate(_textObject, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
                edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(isCurvedEdge, databaseEdge, textObject, _fromNode, _toNode, rotation);

                //if we get time, this should be turned into a function as it recurrs
                _fromNode.GetComponent<FrontEndNode>().to.Add(_toNode);
                _fromNode.GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
                _toNode.GetComponent<FrontEndNode>().from.Add(_fromNode);
            }
            adding = false;   
            Debug.Log("ADDED");
        }
    }
}
