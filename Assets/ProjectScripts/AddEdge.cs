using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class AddEdge : MonoBehaviour
{
    private GameObject _fromNode = null;
    private GameObject _toNode = null;
    public GameObject _edgePrefab = null;
    public GameObject _curvedPrefab = null;
    public GameObject _selfReferencePreFab = null;
    private bool isCurvedEdge = false;
    private bool adding = false;
    public MonoBehaviour cameraController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Input.GetMouseButtonDown(1) && Physics.Raycast(ray, out hitInfo, Mathf.Infinity) && hitInfo.transform.tag == "Node")
        {   
            cameraController.enabled = false;
            _fromNode = hitInfo.collider.gameObject;
            adding = true;
            Debug.Log("ADDING");

        }

        if(Input.GetMouseButtonUp(1)) 
        {
            if (!adding) {
                Debug.Log("NOT ADDING");
                return;
            }
            cameraController.enabled = true;
            Ray ray2 = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo2;

            if (Physics.Raycast(ray2, out hitInfo2, Mathf.Infinity)) 
            {
                _toNode = hitInfo2.collider.gameObject;

                GameObject rightPrefab = _edgePrefab;

                isCurvedEdge = false;

                float rotation = 0;

                if(_toNode == _fromNode) 
                {
                    rightPrefab = _selfReferencePreFab;
                } else if (_fromNode.GetComponent<FrontEndNode>().from.Contains(_toNode) || _toNode.GetComponent<FrontEndNode>().from.Contains(_fromNode)) 
                {
                    rightPrefab = _curvedPrefab;
                    rotation = _fromNode.GetComponent<FrontEndNode>().changeEdge(_toNode, _curvedPrefab);
                    isCurvedEdge = true;
                } 

                GameObject edgeObject = Instantiate(rightPrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
                GraphEdge databaseEdge = new GraphEdge("New Edge", ". . .", _fromNode.GetComponent<FrontEndNode>()._databaseNode, _toNode.GetComponent<FrontEndNode>()._databaseNode);
                StartCoroutine(databaseEdge.CreateInDatabaseCo());
                edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(isCurvedEdge, databaseEdge, null, _fromNode, _toNode, rotation);

                //edgeObject.GetComponent<StoreParentChild>().parent = _fromNode;
                //edgeObject.GetComponent<StoreParentChild>().child = _toNode;
            
                _fromNode.GetComponent<FrontEndNode>().to.Add(_toNode);
                _fromNode.GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
                _toNode.GetComponent<FrontEndNode>().from.Add(_fromNode);
            }
            adding = false;   
            Debug.Log("ADDED");
        }
    }
}
