using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;
using UnityEngine.UI;

public class AddNodeManual : MonoBehaviour
{
    
    public float _distance = 12.0f;                  // how far to put it in front of cam 
    public GameObject _nodePrefab;                  // node type
    private GameObject[] _panels;                    // Panel States to check

    void Start() 
    {
        _panels = GameObject.FindGameObjectsWithTag("Panel");
    }

    // Update is called once per frame
    void Update()
    {
        // depending on the order things are created we may not yet have the panels before start is called,
        // so we do have to poll for this
        if (_panels.Length == 0) _panels = GameObject.FindGameObjectsWithTag("Panel");
    
        // don't run if any of the panels are active
        foreach (GameObject p in _panels) if (p.activeSelf) return; 

        if (Input.GetKeyDown("n") || Input.GetKeyDown("1")) 
        {
           addNewNode();
        }
    }

    public void addNewNode() 
    {
        if(Camera.main.GetComponent<CameraReadOnly>().readOnly == false && Camera.main.GetComponent<DeactivateCamera>().DisableScript.enabled)
        {
            // Create new database node and store it in the newly created gameObject
            // if we get time, this should be turned into a function as it recurrs
            Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward*_distance;
            GameObject nodeObject = Instantiate(_nodePrefab, pos , Camera.main.transform.rotation);
            GraphNode databaseNode = new GraphNode(FindObjectsOfType<LoadGraph>()[0].graph, "New Node", ". . .", ( pos.x, pos.y, pos.z)); 
            StartCoroutine(databaseNode.CreateInDatabase());
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(databaseNode);
        }
    }

}
