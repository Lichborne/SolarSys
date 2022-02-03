using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndNode : MonoBehaviour
{
    // to keep track of the equivalent database node so we know to delete or update it
    public Backend.GraphNode databaseNode = null;
    //the object itself

    [HideInInspector]
    public GameObject nodeObject = null;
}
