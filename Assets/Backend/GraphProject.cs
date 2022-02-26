using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
namespace Backend
{
    public class GraphProject : IGraphRegion
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public List<GraphNode> Nodes { get; private set; } = new List<GraphNode>();
        public List<GraphEdge> Edges { get; private set; }  = new List<GraphEdge>();
        public List<PathRoot> Paths { get; private set; } = new List<PathRoot>();
        public bool IsEmpty { get => !Nodes.Any(); }
        public GraphUser User { get; private set; }

        public GraphProject(GraphUser user, Guid id, string title)
        {
            User = user;
            Id = id;
            Title = title;
        }

        // Please pass in the user instead!!
        public GraphProject(string title) :
            this(new GraphUser("foo.bar@doc.ic.ac.uk"), Guid.NewGuid(), title)
        { }

        public GraphProject(GraphUser user, string title) : 
            this(user, Guid.NewGuid(), title)
        { }

        // Reads nodes, edges and paths from database
        public IEnumerator ReadFromDatabase(Action<GraphProject> processReadProject = null)
        {
            // Read in nodes and edges and process a graph project
            yield return User.Database.ReadNodesFromProjectCo(this, nodesRead => Nodes = nodesRead);
            yield return User.Database.ReadEdgesBetweenNodesCo(this, Nodes, edgesRead => Edges = edgesRead);

            foreach (var edge in Edges)
                edge.Parent.Edges.Add(edge);

            yield return User.Database.ReadEmptyPathRoots(this, pathsRead => Paths = pathsRead);

            if (processReadProject != null)
            {
                yield return "proceed to next frame";
                processReadProject(this);
            }
        }

        public static GraphProject FromJObject(GraphUser user, JObject json)
        {
            string guidString = (string) json["guid"];
            string title = (string) json["title"];
            return new GraphProject(user, Guid.Parse(guidString), title);
        }

        public void AddRelation(GraphNode parent, GraphEdge edge)
        {
            parent.AddEdge(edge);

            // write all this to database
        }

        public IEnumerator CreateInDatabase()
        {
            yield return User.Database.CreateBlankGraphProject(this);

            foreach (GraphNode node in Nodes)
                yield return node.CreateInDatabase();
            
            foreach (GraphEdge edge in Edges)
                yield return edge.CreateInDatabase();
        }
    }
}