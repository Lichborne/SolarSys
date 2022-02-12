using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public DatabaseConnection connection;

    IEnumerator Start()
    {
        connection = new DatabaseConnection();
        yield return StartCoroutine(
            connection.SendWriteTransactions("CREATE (x :RUBBISH {title: 'hi'})")
        );

        yield return StartCoroutine(connection.SendReadTransaction("MATCH (x :RUBBISH) RETURN x.title",
            response => Debug.Log(response)
        ));
    }

    void Update()
    {

    }
}