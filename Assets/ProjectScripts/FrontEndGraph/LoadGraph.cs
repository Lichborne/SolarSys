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
    private List<GameObject> _graphNodes = new List<GameObject>();

    // A list to help keep track of the Edges while setting up the visualization initially
    private List<GameObject> _graphEdges = new List<GameObject>();

    private AuthenticateUser _authenticateUser = null;

    // Start is called before the first frame update
    void Start()
    {
       _authenticateUser = GameObject.FindObjectOfType<AuthenticateUser>();
    }

    // load the relevant project 
    public void LoadProject(string userEmail, string projectTitle) 
    {   
        clearSpace(); // clear obejcts in world space when loading somethig else

        graph = new GraphProject(userEmail, projectTitle);
        StartCoroutine(graph.ReadFromDatabase(displayProject));
    }

    // load a given path currently selected
    public void LoadPath(PathRoot selectedPath) 
    {
        clearSpace(); // clear obejcts in world space when loading somethig else
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
            _graphNodes.Add(nodeObject);
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
                isCurvedEdge = false; // defensive
                // if there is already such a self reference, we need to do a changeEdge to arrange edges appropriately
                if (_graphNodes[parentIndex].GetComponent<FrontEndNode>().from.Contains(_graphNodes[childIndex]) || 
                        _graphNodes[childIndex].GetComponent<FrontEndNode>().from.Contains(_graphNodes[parentIndex])) 
                {
                     _graphNodes[parentIndex].GetComponent<FrontEndNode>().changeEdge(_graphNodes[childIndex], _selfReferencePreFab);
                }

            } 
            else if (_graphNodes[parentIndex].GetComponent<FrontEndNode>().from.Contains(_graphNodes[childIndex]) || 
                        _graphNodes[childIndex].GetComponent<FrontEndNode>().from.Contains(_graphNodes[parentIndex])) 
            {
                rightPrefab = _curvedPrefab;
                _graphNodes[parentIndex].GetComponent<FrontEndNode>().changeEdge(_graphNodes[childIndex], _curvedPrefab);
                isCurvedEdge = true;
            } 
            
            // this might be preferable as as eparate function as it recurs; however, it only recurs across files, and always with variations, so we decided to forgo this
            GameObject textObject = Instantiate(_textObject, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            GameObject edgeObject = Instantiate(rightPrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(isCurvedEdge, databaseEdge, textObject, _graphNodes[parentIndex], _graphNodes[childIndex], 0);
            ChangeText.ChangeInputFieldText(edgeObject.GetComponent<FrontEndEdge>()._textObject, edgeObject.GetComponent<FrontEndEdge>()._databaseEdge.Title);

            //if we get time, this should be turned into a function as it recurrs
            _graphNodes[parentIndex].GetComponent<FrontEndNode>().to.Add(_graphNodes[childIndex]);
            _graphNodes[parentIndex].GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            _graphNodes[childIndex].GetComponent<FrontEndNode>().from.Add(_graphNodes[parentIndex]);
        }

    }

    // clear space of objects when summoning a different project or path
    private void clearSpace() {

        foreach (GameObject o in GameObject.FindObjectsOfType<GameObject>()) {
            if (o.tag == "Node" || o.tag == "Edge" || o.tag == "Text") {
                UnityEngine.Object.Destroy(o);
            }
        }
        _graphNodes.Clear();
        _graphEdges.Clear();

    }

}