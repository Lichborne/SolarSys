using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Backend;
using Newtonsoft.Json.Linq;

public class Test : MonoBehaviour
{
    public DatabaseConnection connection;

    List<GraphNode> GraphNodes;

    void Start()
    {
        connection = new DatabaseConnection();
        GraphProject project = new GraphProject();
        PathRoot path = new PathRoot(project, "new pathy", "hi");

        GraphNode parent = project.Nodes.Where(node => node.Edges.Any()).First();
        GraphNode child = parent.Edges.First().Child;
        
        Debug.Log($"Adding parent {parent} and child {child} to path");
        path.AddNode(parent);
        path.AddNode(child);
        StartCoroutine(path.CreateInDatabase());

        foreach (GraphNode node in path.PathNodes)
            Debug.Log($"Path now has node {node}");
        
        GraphProject newProject = path.AsGraphProject("newy graphy");
        newProject.CreateInDatabase();
        Debug.Log("Saved path as graphproject in database");

        /*
        StartCoroutine(connection.SendWriteTransactions("CREATE (x :RUBBISH {title: 'rubbish'})"));

        StartCoroutine(connection.SendReadTransaction("MATCH (parent :NODE) -[edge :LINK]-> (child :NODE) return parent, edge, child", 
            entries => 
            {
                Debug.Log($"Start() found {entries.Count} entries\n");
                foreach (Dictionary<string, JToken> entry in entries)
                {
                    Debug.Log("Start() found a new entry");
                    foreach (string key in entry.Keys)
                        Debug.Log($"Start() found {key} = {entry[key]}");
                }
            }
        )); */
    }

    // void processGraphEdges(List<GraphEdge> edges)
    // {
    //     foreach (GraphEdge edge in edges)
    //     {
    //         Debug.Log($"Processing edge: {edge.Title}");
    //     }
    // }

    // void saveGraphNodes(List<GraphNode> nodes)
    // {
    //     GraphNodes = nodes;
    // }

    // void processLogNodes(List<LogNode> nodes)
    // {
    //     // Debug.Log(nodes[0].Body);
    //     foreach (LogNode node in nodes)
    //     {
    //         Debug.Log($"Timestamp is {node.TimeStamp}");
    //     }
    // }

    // void prcoessGraphNodes(List<GraphNode> nodes)
    // {
    //     // Debug.Log(nodes[0].Body);
    //     foreach (GraphNode node in nodes)
    //     {
    //         Debug.Log($"Timestamp is {node.Title}");
    //     }
    // }
    void Update()
    {

    }

    void updateTitleTest()
    {

    }
}