using System;
using System.Collections.Generic;

class NodeTree
{
    // address of all children nodes 
    public List<NodeTree> ChildNodes = new List<NodeTree>();

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
        if (ChildNodes.find())
        {
            Chu
        }
    }

    public bool isLeafNode()
    {
        if (this.ChildNodes.Count == 0)
        {
            return true;
        }
        return false;
    }






}