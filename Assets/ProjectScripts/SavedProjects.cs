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
            database.ReadAllProjectTitlesAttachedToUserCo(userEmail, DisplayProjectTitles)
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

    private void DisplayProjectTitles(List<string> projectTitles)
    {
        foreach (string title in projectTitles)
        {
            AddProject(title, savedProjectContainer, savedProjectsContent);
        }

    }

    //This function is called when we select create new project in pop-up panel
    public void CreateProjectButtonClicked()
    {
        createNewProjectPanel.SetActive(false);
        AddProject(newProjectName.GetComponent<TMP_InputField>().text, savedProjectContainer, savedProjectsContent);

        //Add code here to add a new blank project with given title
    }


    
}
