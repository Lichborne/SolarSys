using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class ChangeText : MonoBehaviour
{
    public static void ChangeInputFieldText(GameObject obj, string message) {

        TMP_Text textmeshPro = obj.GetComponentInChildren<TextMeshProUGUI>();

        if(textmeshPro == null)
            return;

        textmeshPro.text = message;
     }
}