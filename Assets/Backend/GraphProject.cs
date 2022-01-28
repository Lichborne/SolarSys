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

        public string ProjectTitle {get; private set;}
        public string UserEmail {get; private set;}

        private string _uri;
        private string _username;
        private string _password;

        public GraphProject(string userEmail = "foo.bar@doc.ic.ac.uk", string projectTitle = "Test Project", string uri = "bolt://localhost:7687", /*"neo4j://cloud-vm-42-36.doc.ic.ac.uk:7687", */ string dbUsername = "neo4j", string dbPassword = "password")
        {
            UserEmail = userEmail;
            ProjectTitle = projectTitle;
            _uri = uri;
            _username = dbUsername;
            _password = dbPassword;

            using (var database = new DatabaseView(_uri, _username, _password))
            {
                /*
                GraphNode parent = new GraphNode("parenty", "hi there parent", (0, 0, 0));                
                GraphNode child = new GraphNode("childy", "hi there child", (0, 1, 0));
                GraphEdge edge = new GraphEdge("edgy", "body of edge", parent, child);
                
                database.CreateUnlinkedNode(this, parent);
                database.CreateUnlinkedNode(this, child);
                database.CreateParentChildRelationship(parent, edge, child);

                GraphEdge newEdge = new GraphEdge(edge.Id, "new edgy", "body of edgy ne", parent, child);
                database.UpdateEdgeFields(newEdge); */

                _nodes = database.ReadNodesFromProject(UserEmail, ProjectTitle);
                _edges = database.ReadAllEdgesFromProject(UserEmail, ProjectTitle, _nodes);
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