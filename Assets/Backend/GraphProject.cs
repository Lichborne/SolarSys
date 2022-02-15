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
            InitialiseProject();
        }

        public void Dispose()
            => Database.Dispose();

        public void AddRelation(GraphNode parent, GraphEdge edge, GraphNode child)
        {
            parent.AddEdge(edge);

            // write all this to database
        }

        public IEnumerator InitialiseProject()
        {
            yield return Database.ReadNodesFromProjectCo(this, processGraphNodes);
            yield return Database.ReadAllEdgesFromProjectCo(ProjectId, Nodes, processEdges);

            foreach (var edge in Edges)
                edge.Parent.Edges.Add(edge); // why do you make the parent recognise its children at this point, and not when the edge is created?
        }

        private void processGraphNodes(List<GraphNode> graphNodes)
        {
            Nodes = graphNodes;
        }

        private void processEdges(List<GraphEdge> graphEdges)
        {
            Edges = graphEdges;
        }
    }
}