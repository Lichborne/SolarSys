using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Backend;

public class SelectProject : MonoBehaviour
{

    public GameObject savedPathViewsPanel;
    public GameObject savedProjectsPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPathsForProject() 
    {
        GameObject projectContainer = gameObject.transform.parent.gameObject;
        GameObject titleField = projectContainer.transform.Find("Text (TMP)").gameObject;
        string title = titleField.GetComponent<TextMeshProUGUI>().text;
        GraphProject project = null;
        // Debug.Log(title);
        //call function in SavedPathViews and pass in title parameter
        List<GraphProject> projects = savedProjectsPanel.GetComponent<SavedProjects>().user.Projects;
        foreach (GraphProject p in projects)
        {
            if (p.Title == title) {
                project = p;
            }
        }
        savedPathViewsPanel.GetComponent<SavedPathViews>().selectedProject = project;
        savedPathViewsPanel.GetComponent<SavedPathViews>().LoadPathViewsForProject(project);

    }
}
