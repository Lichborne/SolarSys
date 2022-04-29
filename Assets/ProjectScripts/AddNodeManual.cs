using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

public class AddNodeManual : MonoBehaviour
{
    // how far to put it in front of cam 
    public float distance = 12.0f;
    // node type
    public GameObject _nodePrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("n") || Input.GetKeyDown("1")) 
        {
            if(Camera.main.GetComponent<DeactivateCamera>().DisableScript.enabled)
            {
            // Create new database node and store it in the newly created gameObject
            // if we get time, this should be turned into a function as it recurrs
            Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward*distance;
            GameObject nodeObject = Instantiate(_nodePrefab, pos , Camera.main.transform.rotation);
            GraphNode databaseNode = new GraphNode(FindObjectsOfType<LoadGraph>()[0].graph, "New Node", ". . .", ( pos.x, pos.y, pos.z)); 
            StartCoroutine(databaseNode.CreateInDatabaseCo());
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(databaseNode);
            }
        }
    }
}
