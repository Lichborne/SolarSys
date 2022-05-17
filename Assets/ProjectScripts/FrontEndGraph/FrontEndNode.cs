using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndNode : MonoBehaviour
{
    // To keep track of the equivalent database node so we know to delete or update it
    public Backend.GraphNode _databaseNode = null;

    // To keep track of Nodes this node is parent to
    public List<GameObject> to { get; set; } = new List<GameObject>();

    // To keep track of Nodes this node is child to 
    public List<GameObject> from { get; set; } = new List<GameObject>();

    // To keep track of the outgoing edge game objects
    public List<GameObject> edgeOut { get; set; } = new List<GameObject>();

    // Sets the database node that belongs to the front end node
    public void setDatabaseNode(Backend.GraphNode graphNode) 
    {
        _databaseNode = graphNode;
    }

    // Gets database node blonging to the front end node; depricated, because direct access is a lot more convenient
    // and due to the necessity of GetComponent to get from game object to FrontEndNode, any access to the database node
    // connected to that node is sufficiently explicit; This was simply extraneous.
    public Backend.GraphNode getDatabaseNode() 
    {
        return _databaseNode;
    }

    // This function changes all edges that go between two nodes into the appropriate prefarb and rotates them appropriately.
    // If time allows and it can be done without significant additional code gymnastics, ideally, rotations should distributed
    // edges equally no matter which way they face; this is a little inconvenient due to rotations being opposite but there
    // being no difference between the objects otherwise, this is inconvenient to implement, and rahter inconsequential; I myself
    // prefer easily visible separability between edges going one way or the other, which the current solution excels at.
    public float changeEdge(GameObject toNode, GameObject preFab) 
    {
        
        List<GameObject> edgesToReplaceFrom = findAllWithChild(toNode);
        List<GameObject> edgesToReplaceTo = toNode.GetComponent<FrontEndNode>().findAllWithChild(gameObject);
        var edgesToReplace = edgesToReplaceFrom.Concat(edgesToReplaceTo);
        
        // This is the clever bit, distributing rotations on either side of an imaginary circle between two nodes
        int rotationSplit = (360/((edgesToReplace.Count())+1));
        // we keep track of where we are in applying the rotations we get above
        int rotationTracker = 1;

        // Add a new edge with the right prefab, get the relevant characteristics, and then destroy the old one; more straightforward than
        // trying to replace the prefab used (which ends up using instantiate and destroy as well, anyway; we tried)
        foreach (GameObject edge in edgesToReplace) 
        {
            int rotation = rotationSplit*rotationTracker;
            if (rotationTracker > edgesToReplaceFrom.Count()) {
                rotation = -rotation;
            }
            if(edgesToReplace.Count() == 1)
            {
                rotation = 180;
                if (toNode == gameObject) 
                {
                    rotation = 90;
                }
            }

            // Debug.Log($"Rotation = {rotation}, edgestorep = {edgesToReplace.Count()}");

            GameObject edgeObject = Instantiate(preFab, new Vector3(UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10), UnityEngine.Random.Range(-10,10)), Quaternion.identity);
        
            edgeObject.GetComponent<FrontEndEdge>().InstantiateEdge(true,
                                                                    edge.GetComponent<FrontEndEdge>()._databaseEdge, 
                                                                    edge.GetComponent<FrontEndEdge>()._textObject, 
                                                                    edge.GetComponent<FrontEndEdge>()._parent, 
                                                                    edge.GetComponent<FrontEndEdge>()._child, 
                                                                    rotation);
            
            Destroy(edge);
            if (rotationTracker <= edgesToReplaceFrom.Count()) 
            {
                edgeOut.Add(edgeObject);
            }
            else
            {
                toNode.GetComponent<FrontEndNode>().edgeOut.Add(edgeObject);
            }

            rotationTracker +=1;
        }

        // have to keep track of this 
        return 0;
    }

    // Finds all edges which have a certain node as the child they point to. This one is rather pretty.
    private List<GameObject> findAllWithChild(GameObject toNode) 
    {
        List<GameObject> edgesToReplace = new List<GameObject>();
        foreach (GameObject edge in edgeOut) 
        {
            // staying defensive
            if (edge != null) {
                if (edge.GetComponent<FrontEndEdge>()._child == toNode) //&& edge.GetComponent<FrontEndEdge>()._child != edge.GetComponent<FrontEndEdge>()._parent) 
                {
                    edgesToReplace.Add(edge);
                }
            }
        }
        // Have to iterate twie because we cant change the contents of the ist while enumerating
        foreach (GameObject edge in edgesToReplace) 
        {
            edgeOut.Remove(edge);
        }
        return edgesToReplace;
    }
}
