/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Neo4JHelloWorld;

public class Cat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        // Test: printing out ID of each node in database
        using var database = new DatabaseView("bolt://localhost:7687", "neo4j", "password");
        foreach (int id in database.AllNodeIds())
        {
            Debug.Log($"found node with index = {id}");
        }

        // var pageRanker = new PageRanker();
        // foreach (int idx in pageRanker.AllNodeIndexes())
        //     Debug.Log($"found node with index = {idx}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}*/