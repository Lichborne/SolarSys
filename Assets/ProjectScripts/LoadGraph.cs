using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Backend;

public class LoadGraph : MonoBehaviour
{
    public GameObject _planet;

    public GameObject _edgePreFab;

    private int x = 50;

    private int y = 50;

    private int z = 50;

    // Start is called before the first frame update
    void Start()
    {
        Backend.Graph graph = new Backend.Graph();

        List<Node> graphNodes = new List<Node>();
            
        foreach (GraphNode node in graph.Nodes) {
            
            GameObject NewNode = Instantiate(_planet,new Vector3(Random.Range(-x/2, x/2), Random.Range(-y/2, y/2), Random.Range(-z/2, z/2)), Quaternion.identity);
            NewNode.transform.parent = transform;
            ChangeText.ChangeInputFieldText(NewNode, node.Text);
            Node NewNodeScript = NewNode.GetComponent<Node>();
            graphNodes.Add(NewNodeScript);
            NewNodeScript.SetEdgePrefab(_edgePreFab);
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        for (int i = 0; i < graph.Nodes.Count; i++) {
            foreach (GraphEdge edge in graph.Edges) {
                if (edge.Child == graph.Nodes[i]) {
                    int index = (graph.Nodes).IndexOf(edge.Parent);
                    if (index != -1 && index < graphNodes.Count)
                            graphNodes[i].AddEdge(graphNodes[index]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}


