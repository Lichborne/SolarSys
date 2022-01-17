using System;
using System.Collections.Generic;

class NodeTree
{

    // Node id
    public Guid Id { get; private set; } = Guid.NewGuid();


    // address of all children nodes 
    public List<NodeTree> ChildNodes { get; private set; } = new List<NodeTree>();

    // coordinates of the node
    public (double x, double y, double z) Coordinates;

    // node content
    public string Content;


    // methods
    public void AddChildNode(NodeTree childNode)
    {
        ChildNodes.Add(childNode);
    }

    public void RemoveChildNode(NodeTree childNode)
    {
        if (ChildNodes.Remove(childNode))
        {

        }

        // TODO maybe return error?
    }

    public bool IsLeafNode()
    {
        if (this.ChildNodes.Count == 0)
        {
            return true;
        }
        return false;
    }


}