using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Backend;

public class SelectPathView : MonoBehaviour
{
    public GameObject savedPathViewsPanel;
    public GameObject selectedPathViewDisplayText;

    // this function is to set the selectedPathView variable in SavedPathViews.cs and also changed the display text for the selected path view
    public void SetSelectedPath() 
    {
        GameObject projectContainer = gameObject.transform.parent.gameObject;
        GameObject titleField = projectContainer.transform.Find("Text (TMP)").gameObject;
        string title = titleField.GetComponent<TextMeshProUGUI>().text;
        GraphProject selected_project = savedPathViewsPanel.GetComponent<SavedPathViews>().selectedProject;
        PathRoot selected_path = null;
        List<PathRoot> paths = selected_project.Paths;
        foreach (PathRoot p in paths)
        {
            if (p.Title == title) {
                selected_path = p;
            }
        }
        savedPathViewsPanel.GetComponent<SavedPathViews>().selectedPathView = selected_path;
        selectedPathViewDisplayText.GetComponent<TextMeshProUGUI>().text = title;

    }
}
