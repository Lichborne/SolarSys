//Original Code From Tobias J. at https://forum.unity.com/threads/implement-a-drag-and-drop-script-with-c.130515/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Backend;
 
class Drag : MonoBehaviour
{
    public GameObject _edgePrefab;
    public GameObject _nodePrefab;
    public GameObject _selfReferencePreFab;
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.grey;
    private bool dragging = false;
    private float distance;

    private float radius = 36;
 
   
    void OnMouseEnter()
    {
        // GetComponent<Renderer>().material.color = mouseOverColor;
    }
 
    void OnMouseExit()
    {
        // GetComponent<Renderer>().material.color = originalColor;
    }

    void OnMouseOver () {
        if (Input.GetMouseButtonDown(2)){

        Vector3 NewPosition = Vector3.zero;
        
        List<Vector3> hitPoints = new List<Vector3>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            hitPoints.Add(hitCollider.gameObject.transform.position);
        }

        int tries = 0;

        /* better: find furthest based on average distance
        List<Vector3> randomPoints = new List<Vector3>();
        List<float> averageDistances = new List<float>();*/

        while (tries < 100) {
            NewPosition = new Vector3(gameObject.transform.position.x+Random.Range(-radius/6, radius/6), gameObject.transform.position.y+Random.Range(-radius/6, radius/6), gameObject.transform.position.z+Random.Range(-radius/6, radius/6));

            foreach (Vector3 hitPoint in hitPoints) {
                if (Vector3.Distance(NewPosition, hitPoint) < radius/6) {
                    tries +=1;
                    continue;
                }  
            }
            break;
        }

        if(tries == 100) {
            Debug.Log("No available position found");
        }

        // we think this spawns in a new node and tries to save it??
        Backend.GraphNode newDatabaseNode = new GraphNode("New Node", (NewPosition.x, NewPosition.y, NewPosition.z));
        // updated your code, pls fix -- what the hecks going on here???
        GameObject nodeObject = Instantiate(_nodePrefab, NewPosition, Quaternion.identity);
        nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(newDatabaseNode);

        /**/
        GameObject edgeObject = new GameObject();
        Backend.GraphEdge newDatabaseEdge = new GraphEdge("New Edge",  GetComponent<FrontEndNode>().getDatabaseNode(), newDatabaseNode);
        edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(newDatabaseEdge, gameObject, nodeObject);
            
    }
}
 
    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp()
    {
        dragging = false;
    }
 
    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
    }
}
 