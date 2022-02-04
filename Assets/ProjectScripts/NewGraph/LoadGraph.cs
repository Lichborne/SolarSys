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

    public GameObject _selfReferencePreFab;

    private Backend.GraphProject graph = new Backend.Graph();

    private List<GameObject> graphNodes = new List<GameObject>();

    private List<GameObject> graphEdges = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

        foreach (GraphNode node in graph.Nodes) {
            
            Vector3 pos = new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z);

            //custom cosntructors to initialize game obejcts are ill-advised in unity; so initialization is separated
            // into default cosntruction and initialization either via the start method or in imm. succession.
                        
            GameObject nodeObject = Instantiate(_nodePrefab, pos, Quaternion.identity);
            nodeObject.GetComponent<FrontEndNode>().setDatabaseNode(node);
            ChangeText.ChangeInputFieldText(nodeObject, node.Text);
            graphNodes.Add(nodeObject);
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
       
        foreach (GraphEdge edge in graph.Edges) {
            int parentIndex = (graph.Nodes).IndexOf(edge.Parent);
            int childIndex = (graph.Nodes).IndexOf(edge.Child);

            UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

            // this is needed so that edgeObject will be instantiated in the right context
            GameObject rightPrefab = _edgePreFab;

            // if it is a self reference, we need a self reference edge, otherwise a normal edge
            if (parentIndex == childIndex) {
                rightPrefab = _selfReferencePreFab;
            }

            Debug.Log("weeeeee");

            GameObject edgeObject = Instantiate(rightPrefab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);

            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(edge, graphNodes[parentIndex], graphNodes[childIndex]);
        }   
    }

}