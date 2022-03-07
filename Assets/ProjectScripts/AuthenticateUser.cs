using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using Backend;
using System.Linq;
using Newtonsoft.Json.Linq;

public class AuthenticateUser : MonoBehaviour
{
    public GameObject savedProjectsPanel;
    public GameObject loginPanel;
    public GameObject errorMessage;


    string authServer = "https://api-materials.doc.ic.ac.uk/auth/login";
    private IEnumerator coroutine;

    // Start is called before the first frame update
    public void authenticate(){
        GameObject signInBox = gameObject.transform.parent.gameObject;
        Debug.Log($"Running auth for {signInBox.name}");

        GameObject usernameField = signInBox.transform.Find("InputField_Username").gameObject;
        GameObject passwordField = signInBox.transform.Find("InputField_Password").gameObject;
        if(usernameField == null || passwordField == null)
        {
            Debug.Log("Failed to get Gameobjects");
        }
        string user = usernameField.GetComponent<TMP_InputField>().text;
        string password = passwordField.GetComponent<TMP_InputField>().text;
        coroutine = SendAuthRequest(user, password, HandleAuth);
        StartCoroutine(coroutine);


    }
    
    public void HandleAuth(long code){
        Debug.Log($"Web request return code {code} ");
        switch(code)
        {
            case 200:
                // Do successful
                errorMessage.SetActive(false);
                loginPanel.SetActive(false);
                savedProjectsPanel.SetActive(true);
                Camera.main.GetComponent<DeactivateCamera>().activateCamera();
                break;
            case 401:
                // Do not successful
                errorMessage.SetActive(true);

                break;
            default:
                throw new Exception($"Unexpected https return code {code}");
        }
    }


    public IEnumerator SendAuthRequest(string username, string password, Action<long> processResponse)
    {
        string transactionUrl = authServer;
        string queryJson = @"{
           ""username"": """ + username + @""",
           ""password"": """ + password + @"""
        }";

        long response = 0;
        yield return PostRequest(transactionUrl, queryJson, (r) => response = r);

        processResponse(response);
    }

    private IEnumerator PostRequest(string uri, string postJson, Action<long> returnCode)
    {
        using var webRequest = new UnityWebRequest(uri, "POST");

        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Accept","*/*");

        byte[] postData = new System.Text.UTF8Encoding().GetBytes(postJson);
        webRequest.uploadHandler = new UploadHandlerRaw(postData);
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        yield return webRequest.SendWebRequest();
        HandleErrorsFromRequest(uri, webRequest);
        returnCode(webRequest.responseCode);
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
                if(webRequest.responseCode != 401)
                {
                    throw new Exception($"Uri: {uri}: protocol error: \n{webRequest.error}");
                }
                break;
        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
