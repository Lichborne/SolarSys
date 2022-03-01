using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Backend 
{
    public class PathRoot : IGraphRegion
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public GraphProject Project { get; private set; }

        public List<GraphNode> Nodes { get; private set; } = new List<GraphNode>();

        public List<GraphEdge> Edges { get; private set; } = new List<GraphEdge>();

        public bool IsEmpty { get => !Nodes.Any(); }

        public PathRoot(GraphProject project, Guid id, string title, string description)
        {
            Project = project;
            Id = id;
            Title = title;
            Description = description;
        }

        public PathRoot(GraphProject project, string title, string description) :
            this(project, Guid.NewGuid(), title, description)
        { }

        public static PathRoot FromJObject(GraphProject project, JObject json)
        {
            string title = (string) json["title"];
            string description = (string) json["description"];
            Guid guid = Guid.Parse((string) json["guid"]);

            return new PathRoot(project, guid, title, description);
        }

        public void AddNode(GraphNode node)
            => Nodes.Add(node);
        
        public IEnumerator CreateInDatabase()
        {
            yield return Project.User.Database.CreateBlankPathRoot(Project, this);
            foreach (GraphNode node in Nodes)
                yield return Project.User.Database.AddNodeToPath(this, node);
        }

        // uses the nodes and edges from the GraphProject
        private IEnumerator ReadSelfFromDatabase()
        {
            List<Guid> idsOfNodeInPath = new List<Guid>();
            yield return Project.User.Database.ReadNodeIdsInPath(this, nodeIds => idsOfNodeInPath = nodeIds);
            
            Nodes.Clear();
            Edges.Clear();

            foreach (Guid nodeId in idsOfNodeInPath)
            {
                GraphNode node = Project.Nodes.Find(n => n.Id == nodeId);
                Nodes.Add(node);
                Edges.AddRange(node.Edges);
            }
        }

        public IEnumerator ReadFromDatabase(Action<PathRoot> processPathRoot = null)
        {      
            yield return ReadSelfFromDatabase();

            if (processPathRoot != null)
            {
                yield return "waiting for next frame :)";
                processPathRoot(this);
            }
        }

        public IEnumerator ReadFromDatabase(Action<IGraphRegion> processGraphRegion = null)
        {      
            yield return ReadSelfFromDatabase();

            if (processGraphRegion != null)
            {
                yield return "waiting for next frame :)";
                processGraphRegion(this);
            }
        }
        
        public GraphProject AsGraphProject(string projectTitle)
        {
            GraphProject projectCopy = new GraphProject(Project.User, projectTitle);
            
            // creating copies of each old node, keeping track of which old node corresponds to which copy
            Dictionary<GraphNode, GraphNode> oldNodeToCopy = new Dictionary<GraphNode, GraphNode>();

            foreach (GraphNode oldNode in Nodes)
            {
                (float X, float Y, float Z) coordsCopy = oldNode.Coordinates;
                GraphNode copy = new GraphNode(Guid.NewGuid(), projectCopy, oldNode.Title, oldNode.Description, coordsCopy);
                oldNodeToCopy[oldNode] = copy;
            }

            // going over each edge in each old node
            // adding a corresponding path copy to the corresponding node copy
            List<GraphEdge> newEdges = new List<GraphEdge>();
            foreach (GraphNode oldNode in Nodes)
            {
                foreach (GraphEdge oldEdge in oldNode.Edges)
                {
                    GraphNode parentCopy = oldNodeToCopy[oldEdge.Parent];
                    GraphNode childCopy = oldNodeToCopy[oldEdge.Child];
                    GraphEdge edgeCopy = new GraphEdge(Guid.NewGuid(), oldEdge.Title, oldEdge.Description, parentCopy, childCopy);   

                    newEdges.Add(edgeCopy);
                }
            }

            projectCopy.Nodes.AddRange(oldNodeToCopy.Values);
            projectCopy.Edges.AddRange(newEdges);
            return projectCopy;
        }

        public IEnumerator DeleteFromDatabase()
            => Project.User.Database.DeletePath(this);
    }
}