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

    // Start is called before the first frame update
    void Start()
    {

        foreach (GraphNode node in graph.Nodes) {
            
            Vector3 pos = new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z);

            GameObject NewNode = Instantiate(_nodePrefab, pos, Quaternion.identity);

            NewNode.transform.position = pos;

            ChangeText.ChangeInputFieldText(NewNode, node.Text);

            graphNodes.Add(NewNode);
            
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
       
       // hash map to do
        foreach (GraphEdge edge in graph.Edges) {
            int parent_index = (graph.Nodes).IndexOf(edge.Parent);
            int child_index = (graph.Nodes).IndexOf(edge.Child);
            NewEdge graphEdge = graphNodes[parent_index].AddComponent<NewEdge>();
            graphEdge.InstantiateEdge(_edgePreFab, _selfReferencePreFab, graphNodes[parent_index], graphNodes[child_index]);
            }
            
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}