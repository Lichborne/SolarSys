using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Backend;

public class OldLoadGraph : MonoBehaviour
{
    public GameObject _planet;

    public GameObject _edgePreFab;

    public GameObject _selfReferencePreFab;

    private Backend.Graph graph = new Backend.Graph();

    private List<GameObject> graphNodes = new List<GameObject>();

    private List<Node> graphNodeScripts = new List<Node>();

    // Start is called before the first frame update
    void Start()
    {

        foreach (GraphNode node in graph.Nodes) {
            
            GameObject NewNode = new GameObject();
            NewNode = Instantiate(_planet,new Vector3(node.Coordinates.X, node.Coordinates.Y, node.Coordinates.Z), Quaternion.identity);

            ChangeText.ChangeInputFieldText(NewNode, node.Text);

            graphNodes.Add(NewNode);
            
        }
        // for simplicity's sake and to avoid duplicates, we do a separate loop.
        // remember to add self-reference.
        for (int i = 0; i < graph.Nodes.Count; i++) {
            foreach (GraphEdge edge in graph.Edges) {
                if (edge.Child == graph.Nodes[i]) {
                    if (edge.Parent == edge.Child) {
                        GameObject selfReferenceRing = Instantiate(_selfReferencePreFab, new Vector3(0,0,0), Quaternion.identity);
                        selfReferenceRing.transform.parent = graphNodes[i].transform;
                        continue;
                    }
                    int index = (graph.Nodes).IndexOf(edge.Parent);
                    if (index != -1 && index < graph.Nodes.Count) {
                            Node NewNodeScript = graphNodes[i].AddComponent<Node>();
                            NewNodeScript.SetEdgePrefab(_edgePreFab);
                            NewNodeScript.AddEdge(graphNodes[index]);
                            graphNodeScripts.Add(NewNodeScript);
                    }
                }
            }
            foreach (GraphEdge edge in graph.Edges) {
                if (edge.Parent == graph.Nodes[i]) {
                    if (edge.Child == edge.Parent) {
                        GameObject selfReferenceRing = Instantiate(_selfReferencePreFab, new Vector3(0,0,0), Quaternion.identity);
                        selfReferenceRing.transform.parent = graphNodes[i].transform;
                        continue;
                    }
                    int index = (graph.Nodes).IndexOf(edge.Child);
                    if (index != -1 && index < graphNodes.Count) {
                            Node NewNodeScript = graphNodes[i].AddComponent<Node>();
                            NewNodeScript.SetEdgePrefab(_edgePreFab);
                            NewNodeScript.AddEdge(graphNodes[index]);
                            graphNodeScripts.Add(NewNodeScript);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}


            /*TMP_Text textmeshPro = NewNode.GetComponentInChildren<TextMeshProUGUI>();
            textmeshPro.text = (node.Text);
            
            
            GameObject NewNode = new GameObject();
            NewNode = Instantiate(_planet,new Vector3(Random.Range(-x/2, x/2), Random.Range(-y/2, y/2), Random.Range(-z/2, z/2)), Quaternion.identity);*/