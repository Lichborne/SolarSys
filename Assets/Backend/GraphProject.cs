using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    public class GraphProject : IDisposable
    {
        public List<GraphNode> Nodes = new List<GraphNode>();
        public List<GraphEdge> Edges = new List<GraphEdge>();
        public (string UserEmail, string ProjectTitle) ProjectId { get; private set; }
        public DatabaseView Database { get; private set; }
        public GraphProject(string userEmail = "foo.bar@doc.ic.ac.uk", string projectTitle = "Test Project", string dbUri = /* "bolt://localhost:7687" */ "neo4j://cloud-vm-42-36.doc.ic.ac.uk:7687", string dbUsername = "neo4j", string dbPassword = "s3cr3t")
        {
            ProjectId = (userEmail, projectTitle);
            Database = new DatabaseView(dbUri, dbUsername, dbPassword);

            Nodes = Database.ReadNodesFromProject(this);
            Edges = Database.ReadAllEdgesFromProject(ProjectId, Nodes);

            foreach (var edge in Edges)
                edge.Parent.Edges.Add(edge);
        }

        // constructor using coroutines
        // public GraphProject(string userEmail = "foo.bar@doc.ic.ac.uk", string projectTitle = "Test Project", string dbUri = /* "bolt://localhost:7687" */ "neo4j://cloud-vm-42-36.doc.ic.ac.uk:7687", string dbUsername = "neo4j", string dbPassword = "s3cr3t")
        // {
        //     ProjectId = (userEmail, projectTitle);
        //     Database = new DatabaseView(dbUri, dbUsername, dbPassword);

        //     yield return Database.ReadNodesFromProjectCo(this, processNodes);
        //     yield return Database.ReadAllEdgesFromProjectCo(this, Nodes, processEdges);

        //     foreach (var edge in Edges)
        //         edge.Parent.Edges.Add(edge);
        // }

        private void processNodes(List<GraphNode> nodes)
        {
            Nodes = nodes;
        }

        private void processEdges(List<GraphEdge> edges)
        {
            Edges = edges;
        }

        public void Dispose()
            => Database.Dispose();

        public void AddRelation(GraphNode parent, GraphEdge edge, GraphNode child)
        {
            parent.AddEdge(edge);

            // write all this to database
        }
    }
}