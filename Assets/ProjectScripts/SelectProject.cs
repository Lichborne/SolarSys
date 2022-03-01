using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectProject : MonoBehaviour
{

    public GameObject savedPathViewsPanel;
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
        // Debug.Log(title);
        //call function in SavedPathViews and pass in title parameter
        savedPathViewsPanel.GetComponent<SavedPathViews>().selectedProject = title;
        savedPathViewsPanel.GetComponent<SavedPathViews>().LoadPathViewsForProject(title);

    }
}
