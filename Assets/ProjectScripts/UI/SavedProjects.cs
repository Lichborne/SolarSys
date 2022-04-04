using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Backend;

public class SavedProjects : MonoBehaviour
{

    private List<string> projectList;
    public Transform savedProjectsContent;
    public Transform savedProjectContainer;
    public GameObject newProjectName;
    public GameObject createNewProjectPanel;
    public GameObject createdByMeToggle;
    public GameObject sharedWithUserEmail;
    [HideInInspector]
    public GraphUser user;
    [HideInInspector]
    public GraphProject selectedProject;
    [HideInInspector]
    public bool readOnly = false;



    void Start()
    {
        // instantiate new list for project names
        projectList = new List<string>();

        // Coroutine code to display project selections when user logs in
        DatabaseView database = new DatabaseView(); // constructor that doesnt load in Neo4J drivers
        string userEmail = "foo.bar@doc.ic.ac.uk";
        // string userEmail = "balazs.frei@ic.ac.uk";
        user = new GraphUser(userEmail);
        StartCoroutine(
            user.ReadEmptyProjectsOwned(DisplayProjectTitles)
        );
    }
    
    

    // function to add a project to the list and for display
    public void AddProject(string projectName, Transform chosenContainer, Transform ChosenParentPanel)
    {
        projectList.Add(projectName);
        RectTransform savedGraphRectTransform = Instantiate(chosenContainer, ChosenParentPanel).GetComponent<RectTransform>();
        TextMeshProUGUI textComponent = savedGraphRectTransform.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = projectName;
        savedGraphRectTransform.gameObject.SetActive(true);

    }

    // function to iterate through all projects of a user and add them
    public void DisplayProjectTitles(GraphUser user)
    {
        if (createdByMeToggle.GetComponent<Toggle>().isOn)
        {
            foreach (GraphProject project in user.Projects)
            {
                AddProject(project.Title, savedProjectContainer, savedProjectsContent);
            }
        }
        else {
            foreach (GraphProject project in user.ReadOnlyProjects)
            {
                AddProject(project.Title, savedProjectContainer, savedProjectsContent);
            }
        }
    }

    //This function is called when we select create new project in pop-up panel
    public void CreateProjectButtonClicked()
    {
        createNewProjectPanel.SetActive(false);
        string projectTitle = newProjectName.GetComponent<TMP_InputField>().text;
        GraphProject newProject = new GraphProject(user, projectTitle);
        //need to change this, currently new project does not show up immediately on screen
        StartCoroutine(newProject.CreateInDatabase());
        CreatedByMeToggleValueChanged();
        newProjectName.GetComponent<TMP_InputField>().text = "";
    }

    // this function is called then toggle value is changed, or when we want to refresh the list of projects displayed
    public void CreatedByMeToggleValueChanged() 
    {
        ClearProjectDisplay();
        if (createdByMeToggle.GetComponent<Toggle>().isOn) {
            StartCoroutine(
                user.ReadEmptyProjectsOwned(DisplayProjectTitles)
            );
        }
        else 
        {
            StartCoroutine(
                user.ReadEmptyProjectsShared(DisplayProjectTitles)
            );
            Debug.Log("Read Only Projects");
        }
    }

    // function to destroy all objects shown in screen
    private void ClearProjectDisplay()
    {
        foreach(Transform child in savedProjectsContent)
        {
            Destroy(child.gameObject);
        }
    }

    // this function is called when the user clicks share in the ShareWithUserPopUpPanel
    public void ShareProject()
    {
        string userEmail = sharedWithUserEmail.GetComponent<TMP_InputField>().text;
        Debug.Log(userEmail);
        GraphUser user = new GraphUser(userEmail);

        StartCoroutine(
            selectedProject.ShareWith(user)
        );
        
    }

    // function to delete a project
    public void DeleteProject()
    {
        StartCoroutine(
            selectedProject.DeleteFromDatabase(CreatedByMeToggleValueChanged)
        );  
    }

    // function to copy a project
    public void CopyProject()
    {
        string title = "Copy1";
        StartCoroutine(
            selectedProject.CreateCopyInDatabase(user,title)
        );  
        CreatedByMeToggleValueChanged();
    }

    // function to set whether project is read only, called when a path or a project is loaded
    public void setWhetherProjectIsReadOnly()
    {
        if (createdByMeToggle.GetComponent<Toggle>().isOn)
        {
            Camera.main.GetComponent<CameraReadOnly>().readOnly= false;
            Debug.Log("set ReadOnly to false");
        }
        else{
            Camera.main.GetComponent<CameraReadOnly>().readOnly=true;
            Debug.Log("set ReadOnly to true");
        }
    }
}
