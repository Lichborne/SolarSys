using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndNode : MonoBehaviour
{
    // to keep track of the equivalent database node so we know to delete or update it
    private Backend.GraphNode databaseNode { get; set; } = null;

    public List<GameObject> to { get; set; } = new List<GameObject>();
    public List<GameObject> from { get; set; } = new List<GameObject>();
    public List<GameObject> edgeOut { get; set; } = new List<GameObject>();
    public List<GameObject> edgeIn { get; set; } = new List<GameObject>();

    public void setDatabaseNode(Backend.GraphNode graphNode) {
        databaseNode = graphNode;
    }

    public Backend.GraphNode getDatabaseNode() {
        return databaseNode;
    }

    // this function changes all edges that go between two nodes into the appropriate prefarb and rotates them appropriately.

    public void changeEdge(GameObject toNode, GameObject preFab) {
        
        List<GameObject> edgesToReplace = findAllWithChild(toNode);

        int rotationSplit = (360/(edgesToReplace.Count + 1));
        int rotationTracker = 0;

        foreach (GameObject edge in edgesToReplace) {

            GameObject edgeObject = Instantiate(preFab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);

            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(true,
                                                                    edge.GetComponent<FrontEndEdge>()._databaseEdge, 
                                                                    edge.GetComponent<FrontEndEdge>()._textObject, 
                                                                    edge.GetComponent<FrontEndEdge>()._parent, 
                                                                    edge.GetComponent<FrontEndEdge>()._child, 
                                                                    rotationSplit*rotationTracker);
            
            Destroy(edge);



            rotationTracker +=1;
        }
    }

    private List<GameObject> findAllWithChild(GameObject toNode) {
        List<GameObject> edgesToReplace = new List<GameObject>();
        foreach (GameObject edge in edgeOut) {
            if (edge.GetComponent<FrontEndEdge>()._child == toNode) {
                edgesToReplace.Add(edge);
                edgeOut.Remove(edge);
            }
        }
        return edgesToReplace;
    }
}
