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
    public GameObject createNewPathViewPopUp;
    public GameObject controlPanel;
    public GameObject loginPanel;
    public GameObject shareWithUserPanel;
   
    // for TextDisplayPanel
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

    // for TextInputPanel
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

    // for UIPanel
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

    // for SavedProjectsPanel
    public void Toggle_SavedProjects()
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

    // for SavedPathViewsPanel
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

    // for NewProjectPopUpPanel
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

    // for NewPathViewPopUpPanel
    public void Toggle_CreateNewPathViewPopUp()
    {
        if (createNewPathViewPopUp.activeSelf == true) 
        {
            createNewPathViewPopUp.SetActive(false);
        }
        else 
        {
            createNewPathViewPopUp.SetActive(true);
        }
    }

    // for ControlsPanel
    public void Toggle_controlPanel()
    {
        if (controlPanel.activeSelf == true) 
        {
            controlPanel.SetActive(false);
        }
        else 
        {
            controlPanel.SetActive(true);
        }
    }

    //for LoginPanel
    public void Toggle_loginPanel()
    {
        if (loginPanel.activeSelf == true) 
        {
            loginPanel.SetActive(false);
        }
        else 
        {
            loginPanel.SetActive(true);
        }
    }

    // for ShareWithUserPopUpPanel
    public void Toggle_shareWithUserPanel()
    {
        if (shareWithUserPanel.activeSelf == true) 
        {
            shareWithUserPanel.SetActive(false);
        }
        else 
        {
            shareWithUserPanel.SetActive(true);
        }
    }

  
}