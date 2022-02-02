using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndNode : MonoBehaviour
{
    // to keep track of the equivalent database node so we know to delete or update it
    public Backend.GraphNode databaseNode = null;
    //the object itself
    public GameObject nodeObject = null;

    public void InstantiateNode(GameObject _nodePrefab, Backend.GraphNode graphNode, Vector3 pos, Quaternion rotation) {
        databaseNode = graphNode;
        nodeObject = Instantiate(_nodePrefab, pos, rotation);
    }
}
