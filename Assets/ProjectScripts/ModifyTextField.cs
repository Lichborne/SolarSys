using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ModifyTextField : MonoBehaviour
{
    public string textEntry;
    public GameObject inputField;
    // Start is called before the first frame update

    public void StoreInput()
    {
        textEntry = inputField.GetComponent<TMPro.TextMeshProUGUI>().text;
        Debug.Log("Text input = " + textEntry);
    }

    // private void getNodeOfSelectedPlanet()
    // {

    // }

    // private void
}
