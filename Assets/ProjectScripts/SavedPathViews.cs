using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavedPathViews : MonoBehaviour
{
    private List<string> pathViewList;
    public Transform savedPathViewsContent;
    public Transform savedPathViewContainer;
    public GameObject newPathViewName;
    public GameObject createNewPathViewPanel;




    void Start()
    {
        // savedGraphsList = new List<GameObject>();
        pathViewList = new List<string>();

        //for now put this in Start(), just to test

        // Commenting out Olivia's code
        string pathView1 = "PathView1";
        string pathView2 = "PathView2";
        string pathView3 = "PathView3";
        string pathView4 = "PathView4";
        string pathView5 = "PathView5";
       

        AddPathView(pathView1, savedPathViewContainer, savedPathViewsContent);
        AddPathView(pathView2, savedPathViewContainer, savedPathViewsContent);
        AddPathView(pathView3, savedPathViewContainer, savedPathViewsContent);
        AddPathView(pathView4, savedPathViewContainer, savedPathViewsContent);
        AddPathView(pathView5, savedPathViewContainer, savedPathViewsContent);
       

      
    }

    //not sure how to implement this, need to get which button is clicked, find its parent container and get it's child text component
    public void LoadPathViewsForProject(string pathViewTitle) {

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

    private void DisplayPathViewTitles(List<string> projectTitles)
    {
        foreach (string title in projectTitles)
        {
            AddPathView(title, savedPathViewContainer, savedPathViewsContent);
        }

    }

    //This function is called when we select create new project in pop-up panel
    public void CreatePathViewButtonClicked()
    {
        createNewPathViewPanel.SetActive(false);
        AddPathView(newPathViewName.GetComponent<TMP_InputField>().text, savedPathViewContainer, savedPathViewsContent);

        //Add code here to add a new path view given currently active
        // foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Node"))
        // {   
          
        // }
    }
}
