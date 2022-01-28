using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    public class GraphProject 
    {
        public IReadOnlyList<GraphNode> Nodes 
        {
            get => _nodes.AsReadOnly();
        }
        public IReadOnlyList<GraphEdge> Edges 
        {
            get => _edges.AsReadOnly();
        }

        private List<GraphNode> _nodes = new List<GraphNode>();
        private List<GraphEdge> _edges = new List<GraphEdge>();

        private string _uri;
        private string _username;
        private string _password;

        public GraphProject(string userEmail = "foo.bar@doc.ic.ac.uk", string projectTitle = "Test Project", string uri = "bolt://localhost:7687", /*"neo4j://cloud-vm-42-36.doc.ic.ac.uk:7687", */ string dbUsername = "neo4j", string dbPassword = "password")
        {
            _uri = uri;
            _username = dbUsername;
            _password = dbPassword;

            using (var database = new DatabaseView(_uri, _username, _password))
            {
                _nodes = database.ReadNodesFromProject(userEmail, projectTitle);
                _edges = database.ReadAllEdgesFromProject(userEmail, projectTitle, _nodes);
            }

            foreach (var edge in _edges)
                edge.Parent.AddEdge(edge);
        }

        public void AddRelation(GraphNode parent, GraphEdge edge, GraphNode child)
        {
            parent.AddEdge(edge);

            // write all this to database
        }
    }
}