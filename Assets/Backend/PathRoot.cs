using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using System.Collections;
using static Backend.StringExtensions;

namespace Backend 
{
    public class PathRoot 
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public GraphProject Project {get; private set; }

        public List<GraphNode> PathNodes { get; private set; }

        public PathRoot(GraphProject project, Guid id, string title, string description)
        {
            Project = project;
            Id = id;
            Title = title;
            Description = description;
        }

        public IEnumerator AddNode(GraphNode node)
        {
            PathNodes.Add(node);
            yield return Project.Database.AddNodeToPath(this, node);
        }

        public IEnumerator ReadNodesInPath()
        {
            List<GraphNode> nodes = new List<GraphNode>();
            yield return Project.Database.ReadGraphNodesInPath(this, nodesRead => nodes = nodesRead);
            
            PathNodes.AddRange(nodes);
        }
        
        public void Export()
        {
            // TODO
        }
    }
}