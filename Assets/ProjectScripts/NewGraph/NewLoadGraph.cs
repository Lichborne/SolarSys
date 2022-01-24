using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Backend;

public class NewLoadGraph : MonoBehaviour
{
    public GameObject _nodePrefab;

    public GameObject _edgePreFab;

    public GameObject _selfReferencePreFab;

    private Backend.Graph graph = new Backend.Graph();

    private List<GameObject> graphNodes = new List<GameObject>();

    private List<NewEdge> graphEdges = new List<NewEdge>();

    private int x = 0;

    // Start is called before the first frame update
    void Start()
    {

        foreach (GraphNode node in graph.Nodes) {
            
            
            GameObject NewNode = Instantiate(_nodePrefab, new Vector3(0,0,x)/*+new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z)*/, Quaternion.identity);

            ChangeText.ChangeInputFieldText(NewNode, node.Text);

            graphNodes.Add(NewNode);

            x+=3;
            
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
       
       // hash map to do
        foreach (GraphEdge edge in graph.Edges) {
            int parent_index = (graph.Nodes).IndexOf(edge.Parent);
            int child_index = (graph.Nodes).IndexOf(edge.Child);
            NewEdge graphEdge = graphNodes[child_index].AddComponent<NewEdge>();
            graphEdge.InstantiateEdge(_edgePreFab, _selfReferencePreFab, graphNodes[parent_index], graphNodes[child_index]);
            }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}