using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Backend;

public class SavedGraphs : MonoBehaviour
{

    private List<string> projectList;
    private List<string> pathViewList;
    public Transform savedProjectsPanel;
    public Transform savedPathViewsPanel;
    public Transform savedProjectContainer;
    public Transform savedPathContainer;
    public TextMeshProUGUI pathViewText;
    public GameObject newProjectName;
    public GameObject createNewProjectPanel;



    
    void Start() 
    {
        // savedGraphsList = new List<GameObject>();
        projectList = new List<string>();
        pathViewList = new List<string>();

        //for now put this in Start(), just to test

        // Commenting out Olivia's code
        string project1 = "Project1";
        string project2 = "Project2";
        string project3 = "Project3";
        string project4 = "Project4";
        string project5 = "Project5";
        string project6 = "Project6";
        string project7 = "Project7";
        string project8 = "Project8";
        string project9 = "Project9";
        string project10 = "Project10";
        string project11 = "Project11";
        string project12 = "Project12";

        AddProject(project1, savedProjectContainer, savedProjectsPanel);
        AddProject(project2, savedProjectContainer, savedProjectsPanel);
        AddProject(project3, savedProjectContainer, savedProjectsPanel);
        AddProject(project4, savedProjectContainer, savedProjectsPanel);
        AddProject(project5, savedProjectContainer, savedProjectsPanel);
        AddProject(project6, savedProjectContainer, savedProjectsPanel);
        AddProject(project7, savedProjectContainer, savedProjectsPanel);
        AddProject(project8, savedProjectContainer, savedProjectsPanel);
        AddProject(project9, savedProjectContainer, savedProjectsPanel);
        AddProject(project10, savedProjectContainer, savedProjectsPanel);
        AddProject(project11, savedProjectContainer, savedProjectsPanel);
        AddProject(project12, savedProjectContainer, savedProjectsPanel);

        //  //for now put this in contructor, just to test
        // string pathView1 = "PathView1";
        // string pathView2 = "PathView2";
        // string pathView3 = "PathView3";
        // AddPathView(pathView1);
        // AddPathView(pathView2);
        // AddPathView(pathView3);
        // RefreshSavedGraphs(pathViewList, savedPathContainer, savedPathViewsPanel, pathViewText); 

        // Adding my own Coroutine code
        DatabaseView database = new DatabaseView(); // constructor that doesnt load in Neo4J drivers
        string userEmail = "foo.bar@doc.ic.ac.uk";
        StartCoroutine(
            database.ReadUsersProjectTitlesCo(userEmail, DisplayProjectTitles)
        );
    }

    //function to append to list of graphs
    public void AddProject(string newProjectName, Transform chosenContainer, Transform ChosenParentPanel) 
    {
        projectList.Add(newProjectName);
        RectTransform savedGraphRectTransform = Instantiate(chosenContainer, ChosenParentPanel).GetComponent<RectTransform>();
        TextMeshProUGUI textComponent = savedGraphRectTransform.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = newProjectName;
        savedGraphRectTransform.gameObject.SetActive(true);
      
    }

    private void DisplayProjectTitles(List<string> projectTitles)
    {
        // someone pls remove these debug.logs
        Debug.Log($"Writing {projectTitles.Count} project titles");
        foreach (string title in projectTitles) 
        {
            Debug.Log($"Writing project title {title}");
            AddProject(title, savedProjectContainer, savedProjectsPanel);
        }
        
    }

    //function to append to list of graphs
    public void AddPathView(string newPathView) 
    {
        pathViewList.Add(newPathView);
    }



    //This function is called when we select create new project in pop-up panel
    public void CreateProjectButtonClicked()
    {   
        createNewProjectPanel.SetActive(false);
        AddProject(newProjectName.GetComponent<TMP_InputField>().text, savedProjectContainer, savedProjectsPanel);
    }
}
