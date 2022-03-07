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
        // savedGraphsList = new List<GameObject>();
        projectList = new List<string>();

        //for now put this in Start(), just to test

        // Commenting out Olivia's code
        // string project1 = "Project1";
        // string project2 = "Project2";
        // string project3 = "Project3";
        // string project4 = "Project4";
        // string project5 = "Project5";
        // string project6 = "Project6";
        // string project7 = "Project7";
        // string project8 = "Project8";
        // string project9 = "Project9";
        // string project10 = "Project10";
        // string project11 = "Project11";
        // string project12 = "Project12";

        // AddProject(project1, savedProjectContainer, savedProjectsPanel);
        // AddProject(project2, savedProjectContainer, savedProjectsPanel);
        // AddProject(project3, savedProjectContainer, savedProjectsPanel);
        // AddProject(project4, savedProjectContainer, savedProjectsPanel);
        // AddProject(project5, savedProjectContainer, savedProjectsPanel);
        // AddProject(project6, savedProjectContainer, savedProjectsPanel);
        // AddProject(project7, savedProjectContainer, savedProjectsPanel);
        // AddProject(project8, savedProjectContainer, savedProjectsPanel);
        // AddProject(project9, savedProjectContainer, savedProjectsPanel);
        // AddProject(project10, savedProjectContainer, savedProjectsPanel);
        // AddProject(project11, savedProjectContainer, savedProjectsPanel);
        // AddProject(project12, savedProjectContainer, savedProjectsPanel);



        // Adding my own Coroutine code
        DatabaseView database = new DatabaseView(); // constructor that doesnt load in Neo4J drivers
        string userEmail = "foo.bar@doc.ic.ac.uk";
        // string userEmail = "balazs.frei@ic.ac.uk";
        user = new GraphUser(userEmail);
        StartCoroutine(
            user.ReadEmptyProjectsOwned(DisplayProjectTitles)
        );
    }
    
    

    //function to append to list of graphs
    public void AddProject(string projectName, Transform chosenContainer, Transform ChosenParentPanel)
    {
        projectList.Add(projectName);
        RectTransform savedGraphRectTransform = Instantiate(chosenContainer, ChosenParentPanel).GetComponent<RectTransform>();
        TextMeshProUGUI textComponent = savedGraphRectTransform.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = projectName;
        savedGraphRectTransform.gameObject.SetActive(true);

    }

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
        // AddProject(projectTitle, savedProjectContainer, savedProjectsContent);
        GraphProject newProject = new GraphProject(user, projectTitle);
        StartCoroutine(newProject.CreateInDatabase());
        CreatedByMeToggleValueChanged();
    }

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

    private void ClearProjectDisplay()
    {
        foreach(Transform child in savedProjectsContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShareProject()
    {
        string userEmail = sharedWithUserEmail.GetComponent<TMP_InputField>().text;
        Debug.Log(userEmail);
        GraphUser user = new GraphUser(userEmail);

        StartCoroutine(
            selectedProject.ShareWith(user)
        );
        
    }

    public void DeleteProject()
    {
        StartCoroutine(
            selectedProject.DeleteFromDatabase(CreatedByMeToggleValueChanged)
        );  
    }

    public void CopyProject()
    {
        string title = "Copy1";
        StartCoroutine(
            selectedProject.CreateCopyInDatabase(user,title)
        );  
        CreatedByMeToggleValueChanged();
    }
}
