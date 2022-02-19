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

    IEnumerator Start()
    {

        var connection = new DatabaseConnection();
        GraphProject project = new GraphProject();

        /* --------- Make a new node and update it -----------*/
        // GraphNode attachedNode = new GraphNode(project, "title hello", "description goes here", (0, 0, 0));
        // // StartCoroutine(project.Database.ReadAllEdgesFromProjectCo(""))
        // yield return attachedNode.CreateInDatabaseCo();
        // yield return attachedNode.UpdateTitleCo("New title");
        // Debug.Log("Finished");

        /* ----- Read all nodes from a project --------------- */
        //yield return project.Database.ReadLogNodesFromProjectCo(project, processLogNodes);

        //yield return project.Database.ReadNodesFromProjectCo(project, prcoessGraphNodes);

        /* ----- Read all edges from project ----------------- */
        // yield return project.Database.ReadNodesFromProjectCo(project, saveGraphNodes);
        // yield return project.Database.ReadAllEdgesFromProjectCo(project, GraphNodes, processGraphEdges);

        /* ----- Remove a node from project ---------------- */
        // GraphNode newNode = new GraphNode(project, "new node", "description goes here", (0, 0, 0));

        // yield return newNode.CreateInDatabaseCo();
        // yield return newNode.DeleteFromDatabaseCo();

        // // GraphProject project = new GraphProject();

        // StartCoroutine(connection.SendWriteTransactions("CREATE (x :RUBBISH {title: 'rubbish'})"));

        // StartCoroutine(connection.SendReadTransaction("MATCH (parent :NODE) -[edge :LINK]-> (child :NODE) return parent, edge, child", 
        //     entries => 
        //     {
        //         Debug.Log($"Start() found {entries.Count} entries\n");
        //         foreach (Dictionary<string, JToken> entry in entries)
        //         {
        //             Debug.Log("Start() found a new entry");
        //             foreach (string key in entry.Keys)
        //                 Debug.Log($"Start() found {key} = {entry[key]}");
        //         }
        //     }
        // ));
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