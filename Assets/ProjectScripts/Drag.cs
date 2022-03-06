//Very loosely inspired by Tobias J. at https://forum.unity.com/threads/implement-a-drag-and-drop-script-with-c.130515/.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

// Class for node interactions; dragging nodes around with left mouse button, adding a new node with middle mouse button, and deleting by pressing delete key on hover.  
// Once again, we can only inherit from MonoBehaviour, so although the data members are similar to other classes, we cannot have an abstract class.
// I would also like to avoid touching MonoBehaviour; it'!'s a delicate beast, and central to everything we do in Unity.
class Drag : MonoBehaviour
{
    // Straigh edge prefab
    public GameObject _edgePrefab;

    // Node prefab. On usage, see LodGraph.cs
    public GameObject _nodePrefab;

    // Self reference edge prefab
    public GameObject _selfReferencePreFab;

    // To trackwhether we are dragging at the moment
    private bool dragging = false;

    // aid for dragging
    private float distance;

    // The radius in which we look at other nodes present to avoid when placing new nodes automatically.
    // this is a "magic number", but much as many things in UI, it is a number we arrived at by trial and error
    // and subjective judgement of visuals; there was and is no way around this.
    private float radius = 36;

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

            // We generate random positions within a reasonable rane and see if there are any collisions with 6 blocks, if not, the new node will be placed in a sufficiently far enough position
            // This is a naive solution, but wuite effective, it is quick, and unless someone builds a sphwere of nodes around a node, is guaranteed to work (it will mostly take at most 5 tries,
            // each of which is unmeasurably fst on its own)
            while (tries < 100) 
            {
                NewPosition = new Vector3(gameObject.transform.position.x+Random.Range(-radius/6, radius/6), gameObject.transform.position.y+Random.Range(-radius/6, radius/6), gameObject.transform.position.z+Random.Range(-radius/6, radius/6));

                foreach (Vector3 hitPoint in hitPoints) 
                {
                    if (Vector3.Distance(NewPosition, hitPoint) < radius/6) 
                    {
                        tries +=1;
                        continue;
                    }  
                }
                break;
            }

            // if by some miracle this was impossible, we regretfully do nothing. A practical impossibility.
            if(tries == 100) {
                Debug.Log("No available position found");
            }
            
            // Create new database node and store it in the newly created gameObject
            // if we get time, this should be turned into a function as it recurrs
            GameObject nodeObject = Instantiate(_nodePrefab, NewPosition, Quaternion.identity);
            GraphNode databaseNode = new GraphNode(FindObjectsOfType<LoadGraph>()[0].graph, "New Node", ". . .", (NewPosition.x, NewPosition.y, NewPosition.z)); 
            StartCoroutine(databaseNode.CreateInDatabaseCo());
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(databaseNode);
            
            //if we get time, this should be turned into a function as it recurrs
            GameObject edgeObject = Instantiate(_edgePrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            GraphEdge databaseEdge = new GraphEdge("New Edge", ". . .", gameObject.GetComponent<FrontEndNode>()._databaseNode, databaseNode);
            StartCoroutine(databaseEdge.CreateInDatabaseCo());
            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(false, databaseEdge, null, gameObject, nodeObject, 90);

            //two lines of Olivia's code, if inefficient can refactor later
            //edgeObject.GetComponent<StoreParentChild>().parent = gameObject;
            //edgeObject.GetComponent<StoreParentChild>().child = nodeObject;

            //if we get time, this should be turned into a function as it recurrs
            gameObject.GetComponent<FrontEndNode>().to.Add(nodeObject);
            gameObject.GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            nodeObject.GetComponent<FrontEndNode>().from.Add(gameObject);
        }

        // if we are hovering over a node, and we click delete, it gets deleted, poof. 
        // quite straightforward
        if (Input.GetKeyDown("delete")) 
        {
            
            foreach (GameObject node in gameObject.GetComponent<FrontEndNode>().from) 
            {
                node.GetComponent<FrontEndNode>().to.RemoveAll(item => item == gameObject);

                foreach (GameObject edge in node.GetComponent<FrontEndNode>().edgeOut) 
                {
                    if (edge.GetComponent<FrontEndEdge>()._child == gameObject)
                    {
                        Destroy(edge.GetComponent<FrontEndEdge>()._textObject);
                        Destroy(edge);
                    }
                }

                node.GetComponent<FrontEndNode>().edgeOut.RemoveAll(item => item == null);
            }

            foreach (GameObject edge in gameObject.GetComponent<FrontEndNode>().edgeOut) 
            {
                Destroy(edge.GetComponent<FrontEndEdge>()._textObject);
                Destroy(edge);
            }

            StartCoroutine(gameObject.GetComponent<FrontEndNode>()._databaseNode.DeleteFromDatabaseCo(() => Destroy(gameObject)));
            
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

    // After we have dragged, we up[date the position in the database
    private void updatePlanetPosition() 
    {
        GraphNode attachedNode = gameObject.GetComponent<FrontEndNode>().getDatabaseNode();
        Vector3 currentPosition = transform.position;
        StartCoroutine(attachedNode.UpdateCoordinatesCo((currentPosition.x, currentPosition.y, currentPosition.z)));
    }

}
 