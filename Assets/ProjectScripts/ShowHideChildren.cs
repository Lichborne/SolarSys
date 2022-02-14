using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShowHideChildren : MonoBehaviour
{   
    public GameObject infoDisplay;
    public GameObject textInput;
    public GameObject UI;
    public GameObject savedProjects;
    public GameObject savedPathViews;
    public GameObject createNewGraphPopUp;
   
   
    public void Toggle_infoDisplay()
    {   
        if (infoDisplay.activeSelf == true) 
        {
            infoDisplay.SetActive(false);
        }
        else 
        {
            infoDisplay.SetActive(true);
        }
    }

    public void Toggle_textInput()
    {   
        if (textInput.activeSelf == true) 
        {
            textInput.SetActive(false);
        }
        else 
        {
            textInput.SetActive(true);
        }
    }

    public void Toggle_UI()
    {   
        if (UI.activeSelf == true) 
        {
            UI.SetActive(false);
        }
        else 
        {
            UI.SetActive(true);
        }
    }

    public void Toggle_Projects()
    {
        if (savedProjects.activeSelf == true) 
        {
            savedProjects.SetActive(false);
        }
        else 
        {
            savedProjects.SetActive(true);
        }
    }

    public void Toggle_SavedPathViews()
    {
        if (savedPathViews.activeSelf == true) 
        {
            savedPathViews.SetActive(false);
        }
        else 
        {
            savedPathViews.SetActive(true);
        }
    }

    public void Toggle_CreateNewGraphPopUp()
    {
        if (createNewGraphPopUp.activeSelf == true) 
        {
            createNewGraphPopUp.SetActive(false);
        }
        else 
        {
            createNewGraphPopUp.SetActive(true);
        }
    }

   

  
}
