using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowHideChildren : MonoBehaviour
{   
    public GameObject infoDisplay;
    public GameObject textInput;
    public GameObject UICanvas;

    void Update()
    {
       
        // //exit on esc
        // if (Input.GetKey(KeyCode.Escape))
        // {
        //     infoDisplay.SetActive(false);
        //     textInput.SetActive(false);
        //     UICanvas.SetActive(false);
        // }
        
    }

    private void OnMouseOver() 
    {   
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if(Input.GetMouseButtonDown(0))
            {
                infoDisplay.SetActive(true);
            }

            if(Input.GetMouseButtonDown(1))
            {
                // UIPanel.transform=Input.mousePosition;
                UICanvas.SetActive(true);
            }
        }
        
    }

    public void SelectDisplayInfo()
    {
        UICanvas.SetActive(false);
        infoDisplay.SetActive(true);
        
    }

    public void SelectTextInput()
    {
        UICanvas.SetActive(false);
        textInput.SetActive(true);
        
    }

    public void CloseUI()
    {
        UICanvas.SetActive(false);
    }

    public void CloseInfoDosplay()
    {
        infoDisplay.SetActive(false);
    }

    public void CloseTextInput()
    {
        textInput.SetActive(false);
    }

  
}
