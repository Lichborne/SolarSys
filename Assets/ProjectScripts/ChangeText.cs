using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ChangeText : MonoBehaviour
{
    public void ChangeInputFieldText(string message) {
 
         Text textscript = GameObject.Find("InputField (TMP)").GetComponentInChildren<Text>(); // This will get the script responsable for editing text
         if(textscript == null){Debug.LogError("Script not found");return;}
         textscript.text = message; // This will change the text inside it
     }
}