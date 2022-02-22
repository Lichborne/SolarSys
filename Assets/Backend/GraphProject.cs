using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
namespace Backend
{
    public class GraphProject : IDisposable
    {
        public List<GraphNode> Nodes = new List<GraphNode>();
        public List<GraphEdge> Edges = new List<GraphEdge>();
        public (string UserEmail, string ProjectTitle) ProjectId { get; private set; }
        public DatabaseView Database { get; private set; }

        public GraphUser User { get; private set; }

        public GraphProject(string userEmail = "foo.bar@doc.ic.ac.uk", string projectTitle = "Test Project", string dbUri = "neo4j://cloud-vm-42-36.doc.ic.ac.uk:7687", string dbUsername = "neo4j", string dbPassword = "s3cr3t")
        {
            ProjectId = (userEmail, projectTitle);
            Database = new DatabaseView(dbUri, dbUsername, dbPassword);
        }

        public IEnumerator readNodesAndEdges(Action<GraphProject> processReadProject = null)
        {
            // Read in nodes and edges and process a graph project
            yield return Database.ReadNodesFromProjectCo(this, processNodes);
            yield return Database.ReadAllEdgesFromProjectCo(this, Nodes, processEdges);

            // foreach( var node in Nodes){
            //     Debug.Log("My Nodes " + node); 
            // }
            foreach (var edge in Edges)
                edge.Parent.Edges.Add(edge);

            if (processReadProject != null)
            {
                yield return "proceed to next frame";
                processReadProject(this);
            }
        }


        private void processNodes(List<GraphNode> nodes)
        {
            Nodes = nodes;
        }

        private void processEdges(List<GraphEdge> edges)
        {
            Edges = edges;
        }

        // DO NOT use this unless you know what you're doing !!!
        public GraphProject(GraphUser user, List<GraphNode> nodes, List<GraphEdge> edges) 
        {
            User = user;
            Nodes = nodes;
            Edges = edges;
        }

        public void Dispose()
            => Database.Dispose();

        public void AddRelation(GraphNode parent, GraphEdge edge)
        {
            parent.AddEdge(edge);

            // write all this to database
        }

        public void CreateInDatabase()
        {
            Database.CreateBlankGraphProject(this);

            foreach (GraphNode node in Nodes)
                node.CreateInDatabase();
            
            foreach (GraphEdge edge in Edges)
                edge.CreateInDatabase();
        }
    }
}