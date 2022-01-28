using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickOn : MonoBehaviour
{   
    [SerializeField]
    private Material unselected;
    [SerializeField]
    private Material selected;

    [HideInInspector]
    public bool currentlySelected = false;
    [HideInInspector]
    public bool currentlyHidden = false;

    private MeshRenderer myRend;
    // Start is called before the first frame update
    void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        ClickMe();

    }

    public void ClickMe() 
    {
        if (currentlySelected == false) 
        {
            myRend.material = unselected;
        }
        else
        {
            myRend.material = selected;
        }

    }

    public void HideUnhideMe() 
    {
        if (currentlyHidden == false) 
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

    }
   
}
