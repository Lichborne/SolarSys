using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Backend;

public class LoadGraph : MonoBehaviour
{
    public GameObject _nodePrefab;

    public GameObject _edgePreFab;

    public GameObject _curvedPrefab;

    public GameObject _selfReferencePreFab;

    public GameObject _textObject;

    private Backend.GraphProject graph = new Backend.GraphProject();

    private List<GameObject> graphNodes = new List<GameObject>();

    private List<GameObject> graphEdges = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(graph.readNodesAndEdges(displayProject));
    }

    public void displayProject(GraphProject graphProject)
    {
        foreach (GraphNode node in graphProject.Nodes) {
            
            Vector3 pos = new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z);
            // Debug.Log(node.Title);
            //custom cosntructors to initialize game obejcts are ill-advised in unity; so initialization is separated
            // into default cosntruction and initialization either via the start method or in imm. succession.
                        
            GameObject nodeObject = Instantiate(_nodePrefab, pos, Quaternion.identity);
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(node);
            ChangeText.ChangeInputFieldText(nodeObject, node.Title);
            graphNodes.Add(nodeObject);
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
       
        foreach (GraphEdge databaseEdge in graphProject.Edges) {
            bool isCurvedEdge = false;
            int parentIndex = (graph.Nodes).IndexOf(databaseEdge.Parent);
            int childIndex = (graph.Nodes).IndexOf(databaseEdge.Child);

            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

            // this is needed so that edgeObject will be instantiated in the right context
            GameObject rightPrefab = _edgePreFab;

            // if it is a self reference, we need a self reference edge, otherwise a normal edge
            if (parentIndex == childIndex) {
                rightPrefab = _selfReferencePreFab;
            } 

            if (graphNodes[parentIndex].GetComponent<FrontEndNode>().from.Contains(graphNodes[childIndex])) {
                rightPrefab = _curvedPrefab;
                graphNodes[parentIndex].GetComponent<FrontEndNode>().changeEdge(graphNodes[childIndex], _curvedPrefab);
                isCurvedEdge = true;
            } 

            if (graphNodes[childIndex].GetComponent<FrontEndNode>().from.Contains(graphNodes[parentIndex])) {
                rightPrefab = _curvedPrefab;
                graphNodes[childIndex].GetComponent<FrontEndNode>().changeEdge(graphNodes[parentIndex], _curvedPrefab);
                isCurvedEdge = true;
            }

            GameObject edgeObject = Instantiate(rightPrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);

            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(isCurvedEdge, databaseEdge, _textObject, graphNodes[parentIndex], graphNodes[childIndex], 90);

            //two lines of Olivia's code, if inefficient can refactor later
            edgeObject.GetComponent<StoreParentChild>().parent = graphNodes[parentIndex];
            edgeObject.GetComponent<StoreParentChild>().child = graphNodes[childIndex];

            graphNodes[parentIndex].GetComponent<FrontEndNode>().to.Add(graphNodes[childIndex]);
            graphNodes[parentIndex].GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            graphNodes[childIndex].GetComponent<FrontEndNode>().from.Add(graphNodes[parentIndex]);
            graphNodes[childIndex].GetComponent<FrontEndNode>().edgeIn.Add(edgeObject);
        }
    }
    /*
    public void displayPath(GraphProject graphProject, int pathno)
    {
        PathRoot pathToLoad = graphProject.Paths[pathno];

        foreach (GraphNode node in pathToLoad.Nodes) {
            
            Vector3 pos = new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z);
            // Debug.Log(node.Title);
            //custom cosntructors to initialize game obejcts are ill-advised in unity; so initialization is separated
            // into default cosntruction and initialization either via the start method or in imm. succession.

            GameObject nodeObject = Instantiate(_nodePrefab, pos, Quaternion.identity);
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(node);
            ChangeText.ChangeInputFieldText(nodeObject, node.Title);
            graphNodes.Add(nodeObject);
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
       
        foreach (GraphEdge databaseEdge in pathToLoad.Edges) {
            bool isCurvedEdge = false;
            int parentIndex = (graph.Nodes).IndexOf(databaseEdge.Parent);
            int childIndex = (graph.Nodes).IndexOf(databaseEdge.Child);

            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

            // this is needed so that edgeObject will be instantiated in the right context
            GameObject rightPrefab = _edgePreFab;

            // if it is a self reference, we need a self reference edge, otherwise a normal edge
            if (parentIndex == childIndex) {
                rightPrefab = _selfReferencePreFab;
            } 

            if (graphNodes[parentIndex].GetComponent<FrontEndNode>().from.Contains(graphNodes[childIndex])) {
                rightPrefab = _curvedPrefab;
                graphNodes[parentIndex].GetComponent<FrontEndNode>().changeEdge(graphNodes[childIndex], _curvedPrefab);
                isCurvedEdge = true;
            } 

            if (graphNodes[childIndex].GetComponent<FrontEndNode>().from.Contains(graphNodes[parentIndex])) {
                rightPrefab = _curvedPrefab;
                graphNodes[childIndex].GetComponent<FrontEndNode>().changeEdge(graphNodes[parentIndex], _curvedPrefab);
                isCurvedEdge = true;
            }

            GameObject edgeObject = Instantiate(rightPrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);

            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(isCurvedEdge, databaseEdge, _textObject, graphNodes[parentIndex], graphNodes[childIndex], 90);

            //two lines of Olivia's code, if inefficient can refactor later
            edgeObject.GetComponent<StoreParentChild>().parent = graphNodes[parentIndex];
            edgeObject.GetComponent<StoreParentChild>().child = graphNodes[childIndex];

            graphNodes[parentIndex].GetComponent<FrontEndNode>().to.Add(graphNodes[childIndex]);
            graphNodes[parentIndex].GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            graphNodes[childIndex].GetComponent<FrontEndNode>().from.Add(graphNodes[parentIndex]);
            graphNodes[childIndex].GetComponent<FrontEndNode>().edgeIn.Add(edgeObject);
        }
    }
    */
}