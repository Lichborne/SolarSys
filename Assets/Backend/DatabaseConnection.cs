using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Backend
{
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

                JObject response = null;
                yield return PostRequest(transactionUrl, queryJson, (r) => response = r);
                HandleErrorsFromTransactionResponse(query, response);
                    
                if (!updatedUrls)
                {
                    commitUrl = (string) response["commit"];
                    string transactionNumber = Between(commitUrl, "/tx/", "/commit");
                    transactionUrl += transactionNumber;                
                    updatedUrls = true;
                }
            }

            string commitJson = @"{ 
                ""statements"": []
            }"; 
            JObject commitResponse = null;
            yield return PostRequest(commitUrl, commitJson, r => commitResponse = r);
            HandleErrorsFromTransactionResponse("commit", commitResponse);
        }



        public IEnumerator SendReadTransaction(string query, Action<List<Dictionary<string, JToken>>> processEntries)
        {
            string transactionUrl =  $"http://{databaseUrl}:{portNumber}/db/{databaseName}/tx/commit";
            string queryJson = @"{
                ""statements"": [{
                    ""statement"": "" " + query + @" ""
                }]
            }";

            JObject response = null;
            yield return PostRequest(transactionUrl, queryJson, (r) => response = r);

            HandleErrorsFromTransactionResponse(query, response);

            List<Dictionary<string, JToken>> entries = EntriesFromReadResponse(response);
            processEntries(entries);
        }

        private static List<Dictionary<string, JToken>> EntriesFromReadResponse(JObject response)
        {
            /* response is of the form 
            {
                results: [{
                    columns: ["name of return value a", "name of return value b", ...],
                    data: [ { 
                            row 1: [
                                { object a1 }, { object b1 }, ...
                            ], 
                            row 1 meta
                        }, {
                            row 2: [
                                { object a2 }, { object b2 }, ...
                            ],
                            row 2 meta
                        }
                    ]
                }]
            } */

            /* this is inconvenient, so I'm converting this to a (string -> list of entries) dictionar of the form 
            {
                name of return value a: object a1,
                name of return value b: object b1, ...    
            }, 
            {
                name of return value a: object a2,
                name of return value b: object b2, ...
            }, 
            ... */

            JArray columnsJson = response.SelectToken("results[0].columns") as JArray;
            List<string> columns = columnsJson.Select(col => (string) col).ToList();

            JArray data = response.SelectToken("results[0].data") as JArray;
            List<Dictionary<string, JToken>> entries = new List<Dictionary<string, JToken>>();

            foreach (JToken datum in data)
            {
                JArray row = datum["row"] as JArray;
                Dictionary<string, JToken> entry = new Dictionary<string, JToken>();

                for (int i = 0; i < columns.Count; i++)
                    entry[columns[i]] = row[i];
                
                entries.Add(entry);
            }
            return entries;
        }

        private static void HandleErrorsFromTransactionResponse(string query, JObject responseJson)
        {            
            List<string> errors = responseJson["errors"]
                .Select(entry => entry.ToString())
                .ToList();
            
            if (errors.Any())
            {
                string errorLog = $"Error running {query}\n" + string.Join("\n", errors);
                throw new InvalidOperationException(errorLog);
            }
        }

        private IEnumerator PostRequest(string uri, string postJson, Action<JObject> processJson = null)
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

            if (processJson != null)
            {
                JObject responseJson = JObject.Parse(webRequest.downloadHandler.text);
                processJson(responseJson);
            }
        }

        private IEnumerator GetRequest(string uri, Action<string> processResponse = null)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(uri);

            yield return webRequest.SendWebRequest();
            HandleErrorsFromRequest(uri, webRequest);

            string response = webRequest.downloadHandler.text;
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
}