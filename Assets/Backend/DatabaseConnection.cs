using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;

public class DatabaseConnection
{
    private string databaseUrl;
    private int portNumber;
    private string databaseName;
    private string username;
    private string password;

    public DatabaseConnection(
        string databaseUrl = "cloud-vm-42-36.doc.ic.ac.uk", 
        int portNumber = 7474,
        string databaseName = "neo4j",
        string username = "neo4j",
        string password = "s3cr3t")
    {
        this.databaseUrl = databaseUrl;
        this.portNumber = portNumber;
        this.databaseName = databaseName;
        this.username = username;
        this.password = password;
    }

    public IEnumerator SendWriteTransactions(params string[] queries)
    {
        string transactionUrl = $"http://{databaseUrl}:{portNumber}/db/{databaseName}/tx/";  // will need to end with transaction # after query starts
        string commitUrl = ""; 
        bool updatedUrls = false;

        foreach (string query in queries)
        {
            string queryJson = @"{
            ""statements"": [{ 
                    ""statement"":  "" " + query + @" ""
                }]
            }";

            string response = "";
            yield return PostRequest(transactionUrl, queryJson, (r) => response = r);
            
            var responseJson = JObject.Parse(response);
            HandleErrorsFromTransactionResponse(query, responseJson);
                
            if (!updatedUrls)
            {
                commitUrl = (string) responseJson["commit"];
                string transactionNumber = Between(commitUrl, "/tx/", "/commit");
                transactionUrl += transactionNumber;                
                updatedUrls = true;
            }
        }

        string commitJson = @"{ 
            ""statements"": []
        }"; 
        string commitResponse = "";
        yield return PostRequest(commitUrl, commitJson, (r) => commitResponse = r);
    }


    public IEnumerator SendReadTransaction(string query, Action<JArray> processData = null)
    {
        string transactionUrl =  $"http://{databaseUrl}:{portNumber}/db/{databaseName}/tx/commit";
        string queryJson = @"{
            ""statements"": [{
                ""statement"": "" " + query + @" ""
            }]
        }";

        JObject response = null;
        yield return PostRequest(transactionUrl, queryJson, (r) => response = JObject.Parse(r));

        HandleErrorsFromTransactionResponse(query, response);
        JArray resultsData = response.SelectToken("results[0].data") as JArray;
        if (resultsData == null)
                throw new Exception($"Could not find any results from response with query {query} \nResponse: {response}");

        if (processData != null)
            processData(resultsData);
    }

    private static void HandleErrorsFromTransactionResponse(string query, JObject responseJson)
    {            
        List<string> errors = responseJson["errors"]
            .Select(entry => (string) entry)
            .ToList();
        
        if (errors.Any())
        {
            string errorLog = $"Error running query {query}\n" + string.Join("\n", errors);
            throw new InvalidOperationException(errorLog);
        }
    }

    private IEnumerator PostRequest(string uri, string postJson, Action<string> processResponse = null)
    {
        using var webRequest = new UnityWebRequest(uri, "POST");

        string loginDetails = $"{username}:{password}";
        byte[] loginBytes = System.Text.Encoding.UTF8.GetBytes(loginDetails);
        string loginBytesString = System.Convert.ToBase64String(loginBytes);

        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Accept", "application/json;charset=UTF-8");
        webRequest.SetRequestHeader("Authorization", $"Basic {loginBytesString}");

        byte[] postData = new System.Text.UTF8Encoding().GetBytes(postJson);
        webRequest.uploadHandler = new UploadHandlerRaw(postData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();
        HandleErrorsFromRequest(uri, webRequest);

        string response = webRequest.downloadHandler.text;
        yield return response;
        if (processResponse != null)
            processResponse(response);
    }

    private IEnumerator GetRequest(string uri, Action<string> processResponse = null)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(uri);

        yield return webRequest.SendWebRequest();
        HandleErrorsFromRequest(uri, webRequest);

        string response = webRequest.downloadHandler.text;
        yield return response;
        if (processResponse != null)
            processResponse(response);
    }

    private void HandleErrorsFromRequest(string uri, UnityWebRequest webRequest)
    {
        switch (webRequest.result)
        {
            case UnityWebRequest.Result.ConnectionError:
                throw new Exception($"Uri: {uri}: connection error: \n{webRequest.error}");

            case UnityWebRequest.Result.DataProcessingError:
                throw new Exception($"Uri: {uri}: data processing error: \n{webRequest.error}");

            case UnityWebRequest.Result.ProtocolError:
                throw new Exception($"Uri: {uri}: protocol error: \n{webRequest.error}");
        }
    }


    private static string Before(string body, string section)
        => body.Split(new string[] {section}, StringSplitOptions.None)[0];

    private static string Between(string body, string before, string after)
    {
        string leftOfAfter = body.Split(new string[] { after }, StringSplitOptions.None)[0];
        string rightOfBefore = leftOfAfter.Split(new string[] { before }, StringSplitOptions.None)[1];

        return rightOfBefore;
    }
}