// Very loosely inspired by J., "Tobias, Comment in 'Implement a Drag and Drop Script with C#'", Forums/Unity Community Discussion/Scripting,
// Feb 21, 2012, at https://forum.unity.com/threads/implement-a-drag-and-drop-script-with-c.130515/.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Backend;

// Class for node interactions; dragging nodes around with left mouse button, adding a new node with middle mouse button, and deleting by pressing delete key on hover.  
// Once again, we can only inherit from MonoBehaviour, so although the data members are similar to other classes, we cannot have an abstract class.
// I would also like to avoid touching MonoBehaviour; it's a delicate beast, and central to everything we do in Unity.
class NodeOperations : MonoBehaviour
{
    // The objects the class at some point may have to instantiate; must be public so that it can be set from editor.

    public GameObject _edgePrefab;              // Straight edge prefab
    
    public GameObject _nodePrefab;              // Node prefab. On usage, see LodGraph.cs

    public GameObject _selfReferencePreFab;     // Self reference edge prefab

    // This is the separate text for edges; cannot be a part of the prefabs due to universal scaling being uncircumventable from inside game object
    public GameObject _textObject;              // Text object

    private bool _dragging = false;             // To trackwhether we are dragging at the moment

    private float _distance;                    // aid for dragging

    private GameObject[] _panels;               // Panel States to check

    // The radius in which we look at other nodes present to avoid when placing new nodes automatically.
    // this is a "magic number", but much as many things in UI, it is a number we arrived at by trial and error
    // and subjective judgement of visuals; there was and is no way around this.
    private const float RADIUS = 36;

    // The divisor by which the radius is devided when searching for a good position; derived by trial and error.
    private const float DIVISOR = 6;

    void Start()
    {
        _panels = GameObject.FindGameObjectsWithTag("Panel");
    }

    void OnMouseOver () 
    {
        // depending on the order things are created we may not yet have the panels before start is called,
        // so we do have to poll for this
        if (_panels.Length == 0) _panels = GameObject.FindGameObjectsWithTag("Panel");
    
        // don't run if any of the panels are active
        foreach (GameObject p in _panels) if (p.activeSelf) return; 

        if (Input.GetMouseButtonDown(2) || Input.GetKeyDown("2"))
        {
            Vector3 NewPosition = Vector3.zero;
            
            List<Vector3> hitPoints = new List<Vector3>();

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, RADIUS);

            foreach (var hitCollider in hitColliders)
            {
                hitPoints.Add(hitCollider.gameObject.transform.position);
            }

            int tries = 0;

            // We generate random positions within a reasonable rane and see if there are any collisions with DIVISOR blocks, if not, the new node will be placed in a sufficiently far enough position
            // This is a naive solution, but wuite effective, it is quick, and unless someone builds a sphwere of nodes around a node, is guaranteed to work (it will mostly take at most 5 tries,
            // each of which is unmeasurably fst on its own)
            while (tries < 100) 
            {
                NewPosition = new Vector3(gameObject.transform.position.x +
                    Random.Range(-RADIUS/DIVISOR, RADIUS/DIVISOR), 
                    gameObject.transform.position.y + Random.Range(-RADIUS/DIVISOR, RADIUS/DIVISOR), 
                    gameObject.transform.position.z + Random.Range(-RADIUS/DIVISOR, RADIUS/DIVISOR));
                
                bool goodPosition = true;

                foreach (Vector3 hitPoint in hitPoints) 
                {
                    if (Vector3.Distance(NewPosition, hitPoint) < RADIUS/DIVISOR) 
                    {
                        tries +=1;
                        goodPosition = false;
                        break;
                    }  
                }

                if (goodPosition) {
                    break;
                }
            }

            // if by some miracle this was impossible, we regretfully place the new node in whatever position it happens to be thrown to last; this is practically impossible.
            if(tries == 100) {
                Debug.Log("No good available position found");
            }
            
            // Create new database node and store it in the newly created gameObject
            // if we get time, this should be turned into a function as it recurrs
            GameObject nodeObject = Instantiate(_nodePrefab, NewPosition, Quaternion.identity);
            GraphNode databaseNode = new GraphNode(FindObjectsOfType<LoadGraph>()[0].graph, "New Node", ". . .", (NewPosition.x, NewPosition.y, NewPosition.z)); 
            StartCoroutine(databaseNode.CreateInDatabase());
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(databaseNode);
            
            //if we get time, this should be turned into a function as it recurrs
            GameObject edgeObject = Instantiate(_edgePrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            GraphEdge databaseEdge = new GraphEdge("New Edge", ". . .", gameObject.GetComponent<FrontEndNode>()._databaseNode, databaseNode);
            StartCoroutine(databaseEdge.CreateInDatabaseCo());
            GameObject textObject = Instantiate(_textObject, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(false, databaseEdge, textObject, gameObject, nodeObject, 90);

            //if we get time, this should be turned into a function as it recurrs
            gameObject.GetComponent<FrontEndNode>().to.Add(nodeObject);
            gameObject.GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            nodeObject.GetComponent<FrontEndNode>().from.Add(gameObject);
        }

        // if we are hovering over a node, and we click delete, it gets deleted, poof. 
        // quite straightforward
        if (Input.GetKeyDown("delete")|| Input.GetKeyDown("backspace")) 
        {
            // in all of the below, we make sure the appropriate links between nodes are severed, and appropriate edges deleted           
            foreach (GameObject node in gameObject.GetComponent<FrontEndNode>().from) 
            {
                
                foreach (GameObject edge in node.GetComponent<FrontEndNode>().edgeOut) 
                {   
                    // good to be defensive
                    if (edge != null) {
                        if (edge.GetComponent<FrontEndEdge>()._child == gameObject)
                        {
                            Destroy(edge.GetComponent<FrontEndEdge>()._textObject);
                            Destroy(edge);
                        }
                    }
                }
                
                node.GetComponent<FrontEndNode>().to.RemoveAll(item => item == gameObject);
                node.GetComponent<FrontEndNode>().edgeOut.RemoveAll(item => item == null);
                
            }
            foreach (GameObject node in gameObject.GetComponent<FrontEndNode>().to) 
            {
                node.GetComponent<FrontEndNode>().from.RemoveAll(item => item == gameObject);
            }

            foreach (GameObject edge in gameObject.GetComponent<FrontEndNode>().edgeOut) 
            {
                if (edge != null) {
                    // We need these first two lines to avoid exceptions because Update() is called on a non-null text object which has already been destroyed
                    GameObject objectRef = edge.GetComponent<FrontEndEdge>()._textObject;
                    edge.GetComponent<FrontEndEdge>()._textObject = null;

                    Destroy(objectRef);
                    Destroy(edge);
                }
            }

            StartCoroutine(gameObject.GetComponent<FrontEndNode>()._databaseNode.DeleteFromDatabaseCo(() => Destroy(gameObject)));
            
        }
    }
    
    void OnMouseDown() // On mouse down will be called when ever mouse is pressed down while over the colider
    {      
        // don't run if any of the panels are active
        foreach (GameObject p in _panels) if (p.activeSelf) return; 

        _distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        _dragging = true;
    }
 
    void OnMouseUp()
    { // On mouse u is called whenever the mouse is released from the planet that it was initially dragging
        // don't run if any of the panels are active
        foreach (GameObject p in _panels) if (p.activeSelf) return; 

        _dragging = false;
        updatePlanetPosition();
    }
 
    void Update()
    {
        // don't run if any of the panels are active
        foreach (GameObject p in _panels) if (p.activeSelf) return; 

        if (_dragging)
        { // Get position of input mouse and update planet position accordingly
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(_distance);
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
 