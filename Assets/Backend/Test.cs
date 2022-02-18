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

    void Start()
    {
        connection = new DatabaseConnection();
        // GraphProject project = new GraphProject();

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
        ));
    }

    void Update()
    {
        
    }
}