using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndNode : MonoBehaviour
{
    // to keep track of the equivalent database node so we know to delete or update it
    public Backend.GraphNode _databaseNode = null;

    public List<GameObject> to { get; set; } = new List<GameObject>();
    public List<GameObject> from { get; set; } = new List<GameObject>();
    public List<GameObject> edgeOut { get; set; } = new List<GameObject>();

    public void setDatabaseNode(Backend.GraphNode graphNode) {
        _databaseNode = graphNode;
    }

    public Backend.GraphNode getDatabaseNode() {
        return _databaseNode;
    }

    // this function changes all edges that go between two nodes into the appropriate prefarb and rotates them appropriately.

    public float changeEdge(GameObject toNode, GameObject preFab) {
        
        List<GameObject> edgesToReplaceFrom = findAllWithChild(toNode);
        List<GameObject> edgesToReplaceTo = toNode.GetComponent<FrontEndNode>().findAllWithChild(gameObject);
        var edgesToReplace = edgesToReplaceFrom.Concat(edgesToReplaceTo);

        int rotationSplit = (180/(edgesToReplace.Count() + 1));
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
            if (rotationTracker < edgesToReplaceFrom.Count()) {
                edgeOut.Add(edgeObject);
            } else {
                toNode.GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            }

            rotationTracker +=1;
        }

        return rotationSplit*rotationTracker;
    }

    private List<GameObject> findAllWithChild(GameObject toNode) {
        List<GameObject> edgesToReplace = new List<GameObject>();
        foreach (GameObject edge in edgeOut) {
            if (edge.GetComponent<FrontEndEdge>()._child == toNode && edge.GetComponent<FrontEndEdge>()._child != edge.GetComponent<FrontEndEdge>()._parent) {
                edgesToReplace.Add(edge);
            }
        }
        //have to iterate twie because we cant change the contents of the ist while enumerating
        foreach (GameObject edge in edgesToReplace) {
            edgeOut.Remove(edge);
        }
        return edgesToReplace;
    }
}
