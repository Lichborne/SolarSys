using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Backend;

public class SavedGraphs : MonoBehaviour
{
    //list of saved graphs by a user
    // private List<GameObject> savedGraphsList;
    private List<string> projectList;
    private List<string> pathViewList;
    public Transform savedProjectsPanel;
    public Transform savedPathViewsPanel;
    public Transform savedProjectContainer;
    public Transform savedPathContainer;
    public TextMeshProUGUI projectText;
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
        // string project1 = "Project1";
        // string project2 = "Project2";
        // string project3 = "Project3";
        // string project4 = "Project4";
        // string project5 = "Project5";
        // string project6 = "Project6";
        // string project7 = "Project7";
        // AddProject(project1);
        // AddProject(project2);
        // AddProject(project3);
        // AddProject(project4);
        // AddProject(project5);
        // AddProject(project6);
        // AddProject(project7);
        // Debug.Log(projectList.Count);
        // RefreshSavedGraphs(projectList, savedProjectContainer, savedProjectsPanel, projectText); 

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
    public void AddProject(string newProject) 
    {
        projectList.Add(newProject);
        RefreshSavedGraphs(projectList, savedProjectContainer, savedProjectsPanel, projectText);
    }

    private void DisplayProjectTitles(List<string> projectTitles)
    {
        // someone pls remove these debug.logs
        Debug.Log($"Writing {projectTitles.Count} project titles");
        foreach (string title in projectTitles)
            Debug.Log($"Writing project title {title}");
        
        RefreshSavedGraphs(projectTitles, savedProjectContainer, savedProjectsPanel, projectText); 
    }

    //function to append to list of graphs
    public void AddPathView(string newPathView) 
    {
        pathViewList.Add(newPathView);
        RefreshSavedGraphs(pathViewList, savedPathContainer, savedPathViewsPanel, pathViewText);
    }

    // //function to get list of graphs, used by UISavedGraphs.cs
    // public List<GameObject> GetSavedGraphsList() 
    // {
    //     return savedGraphsList;
    // }

    //  void Update()
    // {   
    //     //On left click
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         Debug.Log(projectList.Count);
    //     }
    // }

    private void RefreshSavedGraphs(List<string> chosenList, Transform chosenContainer, Transform ChosenParentPanel, TextMeshProUGUI chosenTextDisplay) 
    {   
        float x_displacement = 140f;
        float y_displacement = -130f;
        int x = 0;
        int y = 0;
        float savedGraphCellSize = 130f;
        for (int index = 0; index < chosenList.Count; index++) 
        {
            RectTransform savedGraphRectTransform = Instantiate(chosenContainer, ChosenParentPanel).GetComponent<RectTransform>();
            TextMeshProUGUI textComponent = savedGraphRectTransform.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = chosenList[index];
            
            savedGraphRectTransform.gameObject.SetActive(true);
            savedGraphRectTransform.anchoredPosition = new Vector3(x_displacement + x * savedGraphCellSize, y_displacement -y* savedGraphCellSize, 0);

            x++;
            if(x > 4)
            {
                x = 0;
                y++;
            }
        }
    }

    //This function is called when we select create new project in pop-up panel
    public void CreateProjectButtonClicked()
    {   
        createNewProjectPanel.SetActive(false);
        AddProject(newProjectName.GetComponent<TMP_InputField>().text);
        RefreshSavedGraphs(projectList, savedProjectContainer, savedProjectsPanel, projectText);
    }
}
