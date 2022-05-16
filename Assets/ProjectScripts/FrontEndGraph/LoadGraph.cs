using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

using Backend;

// This class is a behaviour class that loads graphs and paths from the backend. Important comments here as relate to identifying important 
// elements.
public class LoadGraph : MonoBehaviour
{   
    // Prefab for the Nodes. We use a larger version, Sphere (of the celestial kind, for the more astute; in the likeness of a neutron star,
    // much like Nidavellir, the star of legend, the power of which was used to forge Mjorlir), but a smaller one is also uncluded amongst the assets, 
    // perfectly set up for inclusion. It is called PlaceablePlanet. It is a gas giant, as can be see from its relative size to Sphere, for those interested.
    public GameObject _nodePrefab;

    // This is the prefab for straight edges
    public GameObject _edgePreFab;

    // This is the prefab for curved edges; due to the necessity to retain the curve, its scaling is different to that of straight prefabs,
    // but we felt this arrangement was preferable to scaling the two similarly and having a lot of edges of wildly varying thickness
    public GameObject _curvedPrefab;

    // There were many ways to represent this, but a a ring seemed as good as any; clean and neat
    public GameObject _selfReferencePreFab;

    // This is the separate text for edges; cannot be a part of the prefabs due to universal scaling being uncircumventable from inside game object
    public GameObject _textObject;

    // The graph project we will be using from the database
    public Backend.GraphProject graph = null;

    // A list to help keep track of the Nodes while setting up the visualization initially
    private List<GameObject> graphNodes = new List<GameObject>();

    // // A list to help keep track of the Edges while setting up the visualization initially
    private List<GameObject> graphEdges = new List<GameObject>();

    private AuthenticateUser authenticateUser = null;

    // Start is called before the first frame update
    void Start()
    {
        // string projectTitle = "Test Project";
        // graph = new GraphProject(projectTitle);
        // // graphProject.Paths will give you all the PathRoots in the GraphProject
        // StartCoroutine(graph.ReadFromDatabase(displayProject));

        // // List<string> pathNames = graphProject.Paths.Select(path => path.Title).ToList(); // gives you all the names of all the paths
        authenticateUser = GameObject.FindObjectOfType<AuthenticateUser>();
    }

    public void LoadProject(string projectTitle) 
    {
        clearSpace();

        graph = new GraphProject(authenticateUser.currentUser, projectTitle);
        StartCoroutine(graph.ReadFromDatabase(displayProject));
    }

    public void LoadPath(PathRoot selectedPath) 
    {
        clearSpace();
        StartCoroutine(selectedPath.ReadFromDatabase(displayProject));
    }

    // graphProject.readNodesAndEdges will call this function when it has finished loading from database
    public void displayProject(IGraphRegion graphProject)
    {
        foreach (GraphNode node in graphProject.Nodes) 
        {
            
            Vector3 pos = new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z);
            // Debug.Log(node.Title);
            
            // custom cosntructors to initialize game obejcts are ill-advised in unity; so initialization is separated
            // into default cosntruction and initialization either via the start method or in imm. succession.
                        
            GameObject nodeObject = Instantiate(_nodePrefab, pos, Quaternion.identity);
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(node);
            ChangeText.ChangeInputFieldText(nodeObject, node.Title);
            graphNodes.Add(nodeObject);
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
       
        foreach (GraphEdge databaseEdge in graphProject.Edges) 
        {
            bool isCurvedEdge = false;
            int parentIndex = graphProject.Nodes.IndexOf(databaseEdge.Parent);
            int childIndex = graphProject.Nodes.IndexOf(databaseEdge.Child);

            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

            // this is needed so that edgeObject will be instantiated in the right context
            GameObject rightPrefab = _edgePreFab;

            // if it is a self reference, we need a self reference edge, otherwise a normal edge
            if (parentIndex == childIndex) 
            {
                rightPrefab = _selfReferencePreFab;
            } 

            if (graphNodes[parentIndex].GetComponent<FrontEndNode>().from.Contains(graphNodes[childIndex]) || 
                        graphNodes[childIndex].GetComponent<FrontEndNode>().from.Contains(graphNodes[parentIndex])) 
            {
                rightPrefab = _curvedPrefab;
                graphNodes[parentIndex].GetComponent<FrontEndNode>().changeEdge(graphNodes[childIndex], _curvedPrefab);
                isCurvedEdge = true;
            }

            //if we get time, this should be turned into a function as it recurrs
            GameObject textObject = Instantiate(_textObject, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            GameObject edgeObject = Instantiate(rightPrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(isCurvedEdge, databaseEdge, textObject, graphNodes[parentIndex], graphNodes[childIndex], 0);
            ChangeText.ChangeInputFieldText(edgeObject.GetComponent<FrontEndEdge>()._textObject, edgeObject.GetComponent<FrontEndEdge>()._databaseEdge.Title);
            //depricated
            //edgeObject.GetComponent<StoreParentChild>().parent = graphNodes[parentIndex];
            //edgeObject.GetComponent<StoreParentChild>().child = graphNodes[childIndex];

             //if we get time, this should be turned into a function as it recurrs
            graphNodes[parentIndex].GetComponent<FrontEndNode>().to.Add(graphNodes[childIndex]);
            graphNodes[parentIndex].GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            graphNodes[childIndex].GetComponent<FrontEndNode>().from.Add(graphNodes[parentIndex]);
        }

    }
    private void clearSpace() {
        foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>()) {
            if (o.tag == "Node" || o.tag == "Edge" || o.tag == "Text") {
                UnityEngine.Object.Destroy(o);
            }
        }

    }

}