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
        if (Input.GetMouseButtonDown(2))
        {
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
            
            // Create new database node and store it in the newly created gameObject
            GameObject nodeObject = Instantiate(_nodePrefab, NewPosition, Quaternion.identity);
            GraphNode databaseNode = new GraphNode(FindObjectsOfType<LoadGraph>()[0].graph, "New Node", ". . .", (NewPosition.x, NewPosition.y, NewPosition.z)); 
            StartCoroutine(databaseNode.CreateInDatabaseCo());
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(databaseNode);
            

            GameObject edgeObject = Instantiate(_edgePrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            GraphEdge databaseEdge = new GraphEdge("New Edge", ". . .", gameObject.GetComponent<FrontEndNode>()._databaseNode, databaseNode);
            StartCoroutine(databaseEdge.CreateInDatabaseCo());
            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(false, databaseEdge, null, gameObject, nodeObject, 90);

            //two lines of Olivia's code, if inefficient can refactor later
            //edgeObject.GetComponent<StoreParentChild>().parent = gameObject;
            //edgeObject.GetComponent<StoreParentChild>().child = nodeObject;
            
            gameObject.GetComponent<FrontEndNode>().to.Add(nodeObject);
            gameObject.GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            nodeObject.GetComponent<FrontEndNode>().from.Add(gameObject);
        }

        if (Input.GetKeyDown("delete")) {
            
            foreach (GameObject node in gameObject.GetComponent<FrontEndNode>().from) 
            {
                node.GetComponent<FrontEndNode>().to.RemoveAll(item => item == gameObject);

                foreach (GameObject edge in node.GetComponent<FrontEndNode>().edgeOut) 
                {
                    if (edge.GetComponent<FrontEndEdge>()._child == gameObject)
                    {
                        //StartCoroutine(edge.GetComponent<FrontEndEdge>()._databaseEdge.DeleteFromDatabaseCo());
                        Destroy(edge);
                    }
                }

                node.GetComponent<FrontEndNode>().edgeOut.RemoveAll(item => item == null);
            }

            foreach (GameObject edge in gameObject.GetComponent<FrontEndNode>().edgeOut) 
            {
                //StartCoroutine(edge.GetComponent<FrontEndEdge>()._databaseEdge.DeleteFromDatabaseCo());
                Destroy(edge);
            }

            StartCoroutine(gameObject.GetComponent<FrontEndNode>()._databaseNode.DeleteFromDatabaseCo());
            Destroy(gameObject);
        }
    }
    
    void OnMouseDown() // On mouse down will be called when ever mouse is pressed down while over the colider
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp()
    { // On mouse u is called whenever the mouse is released from the planet that it was initially dragging
        dragging = false;
        updatePlanetPosition();
    }
 
    void Update()
    {
        if (dragging)
        { // Get position of input mouse and update planet position accordingly
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
        
    }

    private void updatePlanetPosition() 
    {
        GraphNode attachedNode = gameObject.GetComponent<FrontEndNode>().getDatabaseNode();
        Vector3 currentPosition = transform.position;
        StartCoroutine(attachedNode.UpdateCoordinatesCo((currentPosition.x, currentPosition.y, currentPosition.z)));
        Debug.Log("Updating planet " + attachedNode.Title + "'s coordinates to " + currentPosition);
    }

}
 