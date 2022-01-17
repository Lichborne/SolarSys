using System;
using System.Collections.Generic;
namespace Backend
{
    class NodeTree
    {

        public NodeTree((double x, double y, double z) coordinates, string content)
        {
            Coordinates = coordinates;
            Content = content;
            Id = Guid.NewGuid();
        }

        public NodeTree((double x, double y, double z) coordinates, string content, Guid id)
        {
            Coordinates = coordinates;
            Content = content;
            Id = id;
        }

        // Node id
        public Guid Id { get; private set; }


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
}
