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
using UnityEngine.EventSystems;


public class AuthenticateUser : MonoBehaviour
{
    public GameObject savedProjectsPanel;
    public GameObject loginPanel;
    public GameObject errorMessage;
    [HideInInspector]
    public GameObject usernameField;
    EventSystem system;

    [HideInInspector]
    public GraphUser currentUser;


    string authServer = "https://api-materials.doc.ic.ac.uk/auth/login";
    private IEnumerator coroutine;

    void Start ()
    {
        system = EventSystem.current;
         
    }

    void Update() {
        if(loginPanel.activeSelf)
        {
            if(Input.GetKeyDown (KeyCode.Return))
            {
                authenticate();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            
                if (next != null)
                {
                
                    InputField inputfield = next.GetComponent<InputField>();
                    if (inputfield != null)
                        inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
                
                    system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
                }
                //else Debug.Log("next nagivation element not found");
            
            }
        }
    }

    public void authenticate(){
        GameObject signInBox = gameObject.transform.parent.gameObject;
        // Debug.Log($"Running auth for {signInBox.name}");

        usernameField = signInBox.transform.Find("InputField_Username").gameObject;
        GameObject passwordField = signInBox.transform.Find("InputField_Password").gameObject;
        // if(usernameField == null || passwordField == null)
        // {
        //     // Debug.Log("Failed to get Gameobjects");
        // }
        string user = usernameField.GetComponent<TMP_InputField>().text;
        string password = passwordField.GetComponent<TMP_InputField>().text;
        coroutine = SendAuthRequest(user, password, (long code) => HandleAuth(code, user));
        StartCoroutine(coroutine);


    }
    
    public void HandleAuth(long code, string userEmail) {
        // Debug.Log($"Web request return code {code} ");
        switch(code)
        {
            case 200:
                // Do successful
                errorMessage.SetActive(false);
                loginPanel.SetActive(false);
                savedProjectsPanel.SetActive(true);
                Camera.main.GetComponent<DeactivateCamera>().activateCamera();
                currentUser = new GraphUser(userEmail);

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


}
