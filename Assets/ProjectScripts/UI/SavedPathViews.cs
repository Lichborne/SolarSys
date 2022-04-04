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
        pathViewList = new List<string>();
      
    }

    //function to load list of path views for a selected project
    public void LoadPathViewsForProject() {

        ClearPathViewsDisplay();
        selectedProject = savedProjectsPanel.GetComponent<SavedProjects>().selectedProject;
        Debug.Log(selectedProject.Title);
        StartCoroutine(
            selectedProject.ReadFromDatabase(DisplayPathViewTitles)
        );
    }
    
    

    //function add a path view for display on screen
    public void AddPathView(string pathViewName, Transform chosenContainer, Transform ChosenParentPanel)
    {
        pathViewList.Add(pathViewName);
        RectTransform savedGraphRectTransform = Instantiate(chosenContainer, ChosenParentPanel).GetComponent<RectTransform>();
        TextMeshProUGUI textComponent = savedGraphRectTransform.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = pathViewName;
        savedGraphRectTransform.gameObject.SetActive(true);
    }

    // function to iterate through all path views of a project and add them for display
    private void DisplayPathViewTitles(GraphProject project)
    {
        foreach (PathRoot path in project.Paths)
        {
            AddPathView(path.Title, savedPathViewContainer, savedPathViewsContent);
        }

    }

    //This function is called when SavePathView button is clicked in the NewPathViewPopUpPanel
    public void CreatePathViewButtonClicked()
    {
        createNewPathViewPanel.SetActive(false);
        string pathTitle = newPathViewName.GetComponent<TMP_InputField>().text;
        var currentlySelectedNodes = Camera.main.GetComponent<Click>().shownNodes;
        AddPathView(pathTitle, savedPathViewContainer, savedPathViewsContent);

        selectedProject = savedProjectsPanel.GetComponent<SavedProjects>().selectedProject;
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

    // this function is for loading the original project and not a path view
    public void LoadOriginalProject() 
    {
        player.GetComponent<LoadGraph>().LoadProject(selectedProject.Title);
    }

    // this function is for loading the selected path view
    public void LoadSelectedPathView()
    {
        player.GetComponent<LoadGraph>().LoadPath(selectedPathView);
    }

    // this function is to detroy all path views displayed on screen
    private void ClearPathViewsDisplay()
    {
        foreach(Transform child in savedPathViewsContent)
        {
            Destroy(child.gameObject);
        }
    }
}
