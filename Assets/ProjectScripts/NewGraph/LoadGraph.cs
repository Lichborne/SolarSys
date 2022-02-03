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

    private Backend.Graph graph = new Backend.Graph();

    private List<FrontEndNode> graphNodes = new List<FrontEndNode>();

    private List<FrontEndEdge> graphEdges = new List<FrontEndEdge>();

    // Start is called before the first frame update
    void Start()
    {

        foreach (GraphNode node in graph.Nodes) {
            
            Vector3 pos = new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z);

            //custom cosntructors to initialize game obejcts are ill-advised in unity; so initialization is separated
            // into default cosntruction and initialization either via the start method or in imm. succession.
                        
            GameObject nodeObject = Instantiate(_nodePrefab, pos, Quaternion.identity);
            nodeObject.GetComponent<FrontEndNode>().databaseNode = node;
            nodeObject.GetComponent<FrontEndNode>().nodeObject = nodeObject;
            ChangeText.ChangeInputFieldText(nodeObject, node.Text);
            graphNodes.Add(nodeObject.GetComponent<FrontEndNode>());
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
       
       // hash map to do
        foreach (GraphEdge edge in graph.Edges) {
            int parent_index = (graph.Nodes).IndexOf(edge.Parent);
            int child_index = (graph.Nodes).IndexOf(edge.Child);
            FrontEndEdge graphEdge = graphNodes[parent_index].nodeObject.AddComponent<FrontEndEdge>();
            graphEdge.InstantiateEdge(_edgePreFab, _selfReferencePreFab, edge, graphNodes[parent_index].nodeObject, graphNodes[child_index].nodeObject);
            }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}