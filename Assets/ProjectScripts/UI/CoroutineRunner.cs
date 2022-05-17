using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Backend;

public class CoroutineRunner : MonoBehaviour
{
    public void RunCoroutine(IEnumerator coroutineToRun) 
    {
        Debug.Log("RunCoroutine() started");
        StartCoroutine(coroutineToRun);
    }
}