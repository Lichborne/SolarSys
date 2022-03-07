using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Backend;

public class SavedPathViews : MonoBehaviour
{
    private List<string> pathViewList;
    public Transform savedPathViewsContent;
    public Transform savedPathViewContainer;

    [SerializeField]
    public GameObject newPathViewName;
    public GameObject createNewPathViewPanel;
    public GameObject savedPathViewsPanel;
    public GameObject savedProjectsPanel;
    public GameObject player;
    [HideInInspector]
    public GraphProject selectedProject;
    [HideInInspector]
    public PathRoot selectedPathView;
    [HideInInspector]
    public GraphUser user;

    void Start()
    {
        // savedGraphsList = new List<GameObject>();
        pathViewList = new List<string>();

        //for now put this in Start(), just to test

        // // Commenting out Olivia's code
        // string pathView1 = "PathView1";
        // string pathView2 = "PathView2";
        // string pathView3 = "PathView3";
        // string pathView4 = "PathView4";
        // string pathView5 = "PathView5";
       

        // AddPathView(pathView1, savedPathViewContainer, savedPathViewsContent);
        // AddPathView(pathView2, savedPathViewContainer, savedPathViewsContent);
        // AddPathView(pathView3, savedPathViewContainer, savedPathViewsContent);
        // AddPathView(pathView4, savedPathViewContainer, savedPathViewsContent);
        // AddPathView(pathView5, savedPathViewContainer, savedPathViewsContent);
       

      
    }

    //load list of path views for selected project
    public void LoadPathViewsForProject(GraphProject project) {
        // Debug.Log(selectedProject);
        // Debug.Log(project);

        // project.ReadFromDatabase() - how to use
        // user = savedProjectsPanel.GetComponent<SavedProjects>().user;
        // Debug.Log(user.Email);

        StartCoroutine(
            project.ReadFromDatabase(DisplayPathViewTitles)
        );
    }
    
    

    //function to append to list of graphs
    public void AddPathView(string pathViewName, Transform chosenContainer, Transform ChosenParentPanel)
    {
        pathViewList.Add(pathViewName);
        RectTransform savedGraphRectTransform = Instantiate(chosenContainer, ChosenParentPanel).GetComponent<RectTransform>();
        TextMeshProUGUI textComponent = savedGraphRectTransform.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = pathViewName;
        savedGraphRectTransform.gameObject.SetActive(true);

    }

    private void DisplayPathViewTitles(GraphProject project)
    {
        foreach (PathRoot path in project.Paths)
        {
            AddPathView(path.Title, savedPathViewContainer, savedPathViewsContent);
        }

    }

    //This function is called when we select create new project in pop-up panel
    public void CreatePathViewButtonClicked()
    {
        createNewPathViewPanel.SetActive(false);
        string pathTitle = newPathViewName.GetComponent<TMP_InputField>().text;
        var currentlySelectedNodes = Camera.main.GetComponent<Click>().shownNodes;
        AddPathView(pathTitle, savedPathViewContainer, savedPathViewsContent);

        PathRoot path = new PathRoot(selectedProject, pathTitle, "path description");
        Debug.Log("Current Nodes" + currentlySelectedNodes);
        foreach (var gameobject in currentlySelectedNodes) // for each graph node you want to add to the path
        {
            GraphNode attachedNode = gameobject.GetComponent<FrontEndNode>().getDatabaseNode();
            Debug.Log("Adding Node to Path" + attachedNode.Title);
            path.AddNode(attachedNode); // adding the graph node
        }

        player.GetComponent<LoadGraph>().StartCoroutine(path.CreateInDatabase()); // saving the project to the database
    }

    public void LoadOriginalProject() 
    {
        player.GetComponent<LoadGraph>().LoadProject(selectedProject.Title);
    }

    public void LoadSelectedPathView(PathRoot pathView)
    {
        // player.GetComponent<LoadGraph>().LoadPathView(selectedPathView);
        player.GetComponent<LoadGraph>().LoadPath(pathView);
    }
}
