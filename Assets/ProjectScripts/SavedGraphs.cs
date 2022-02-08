using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavedGraphs : MonoBehaviour
{
    //list of saved graphs by a user
    private List<GameObject> savedGraphsList;
    private List<string> projectList;
    public Transform savedGraphsPanel;
    public Transform savedGraphContainer;
    public TextMeshProUGUI textDisplay;

    
    void Start() 
    {
        savedGraphsList = new List<GameObject>();
        projectList = new List<string>();

        //for now put this in contructor, just to test
        string project1 = "Project1";
        string project2 = "Project2";
        string project3 = "Project3";
        string project4 = "Project4";
        string project5 = "Project5";
        string project6 = "Project6";
        string project7 = "Project7";
        AddProject(project1);
        AddProject(project2);
        AddProject(project3);
        AddProject(project4);
        AddProject(project5);
        AddProject(project6);
        AddProject(project7);
        Debug.Log(projectList.Count);
        RefreshSavedGraphs();

    }

    //function to append to list of graphs
    public void AddProject(string newProject) 
    {
        projectList.Add(newProject);
        RefreshSavedGraphs();
    }

    // //function to get list of graphs, used by UISavedGraphs.cs
    // public List<GameObject> GetSavedGraphsList() 
    // {
    //     return savedGraphsList;
    // }

     void Update()
    {   
        //On left click
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(projectList.Count);
        }
    }

    private void RefreshSavedGraphs() 
    {   
        float x_displacement = 140f;
        float y_displacement = -130f;
        int x = 0;
        int y = 0;
        float savedGraphCellSize = 130f;
        for (int index = 0; index < projectList.Count; index++) 
        {
            
            RectTransform savedGraphRectTransform = Instantiate(savedGraphContainer, savedGraphsPanel).GetComponent<RectTransform>();
            savedGraphRectTransform.gameObject.SetActive(true);
           
            textDisplay.text = projectList[index];
           
            
            savedGraphRectTransform.anchoredPosition = new Vector3(x_displacement + x * savedGraphCellSize, y_displacement -y* savedGraphCellSize, 0);
            // savedGraphsList.Add(savedGraph);
            x++;
            if(x > 4)
            {
                x = 0;
                y++;
            }

        }

    }

}
