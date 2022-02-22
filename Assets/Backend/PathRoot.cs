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
        
        // Doesnt actually work yet. Waiting for Rowen + Josh to restructure GraphProject
        public GraphProject CopyToNewProject(string projectTitle)
        {
            // replace this with something sensible
            GraphProject projectCopy = new GraphProject(userEmail: Project.ProjectId.UserEmail, projectTitle: projectTitle);
            
            // creating copies of each old node, keeping track of which old node corresponds to which copy
            Dictionary<GraphNode, GraphNode> oldNodeToCopy = new Dictionary<GraphNode, GraphNode>();

            foreach (GraphNode oldNode in PathNodes)
            {
                (float X, float Y, float Z) coordsCopy = oldNode.Coordinates;
                GraphNode copy = new GraphNode(Guid.NewGuid(), projectCopy, oldNode.Title, oldNode.Description, coordsCopy);
                oldNodeToCopy[oldNode] = copy;
            }

            // going over each edge in each old node
            // adding a corresponding path copy to the corresponding node copy
            List<GraphEdge> newEdges = new List<GraphEdge>();
            foreach (GraphNode oldNode in PathNodes)
            {
                foreach (GraphEdge oldEdge in oldNode.Edges)
                {
                    GraphNode parentCopy = oldNodeToCopy[oldEdge.Parent];
                    GraphNode childCopy = oldNodeToCopy[oldEdge.Child];
                    GraphEdge edgeCopy = new GraphEdge(Guid.NewGuid(), oldEdge.Title, oldEdge.Description, parentCopy, childCopy);   

                    newEdges.Add(edgeCopy);
                }
            }

            // TODO: 
            // set oldNodeToCopy.Values() as projectCopy's graph nodes
            // set newEdges as projectCopy's list of edges
            return projectCopy;
        }
    }
}