using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndNode : MonoBehaviour
{
    // to keep track of the equivalent database node so we know to delete or update it
    private Backend.GraphNode databaseNode = null;

    public void setDatabaseNode(Backend.GraphNode graphNode) {
        databaseNode = graphNode;
    }

    public Backend.GraphNode getDatabaseNode() {
        return databaseNode;
    }
}
