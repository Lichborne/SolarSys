using System;
using Neo4j.Driver;
using static Backend.StringExtensions;

using Newtonsoft.Json.Linq;

namespace Backend
{
    public class GraphEdge : IEquatable<GraphEdge>
    {
        public string Title { get; private set; }
        public string Description { get; private set; }

        public Guid Id { get; private set; }

        public GraphProject Project { get; private set; }

        public GraphNode Parent { get; private set; }
        public GraphNode Child { get; private set; }

        public GraphEdge(Guid id, string title, string body, GraphNode parent, GraphNode child)
        {
            Title = title;
            Description = body;
            Id = id;
            Parent = parent;
            Child = child;
        }

        public GraphEdge(string title, string body, GraphNode parent, GraphNode child) :
            this(Guid.NewGuid(), title, body, parent, child)
        { }

        public GraphNode GetAttachedNode(GraphNode node) // return node on other side of edge
        {
            if (node == Parent)
            {
                return Child;
            }
            else if (node == Child)
            {
                return Parent;
            }
            else
            {
                throw new ArgumentException("Node not attached to edge");
            }
        }

        public static GraphEdge FromIRelationship(IRelationship dbRelationship, GraphNode parent, GraphNode child)
        {
            string title = dbRelationship.Properties["title"].As<string>();
            string description = dbRelationship.Properties["description"].As<string>();
            string guidText = dbRelationship.Properties["guid"].As<string>();
            return new GraphEdge(Guid.Parse(guidText), title, description, parent, child);
        }

        public static GraphEdge FromJObject(JObject obj, GraphNode parent, GraphNode child)
        {
            string title = obj["title"].As<string>();
            string description = obj["description"].As<string>();
            string guidText = obj["guid"].As<string>();
            return new GraphEdge(Guid.Parse(guidText), title, description, parent, child);
        }

        // Writes the "parent -[edge] -> child" relationship stored in this edge to the database
        public void CreateInDatabase()
            => Project.Database.CreateParentChildRelationship(Parent, this, Child);

        // Updates the title of the edge, writing change to database
        public void UpdateTitle(string title)
        {
            Title = title;
            Project.Database.UpdateEdgeTitle(this, title);
        }

        // updates the description of the edge, writing change to database
        public void UpdateDescription(string description)
        {
            Description = description;
            Project.Database.UpdateEdgeDescription(this, description);
        }

        // deletes the edge from the database. does not affect the edge's parent or child.
        public void DeleteFromDatabase()
            => Project.Database.DestroyEdge(this);


        public override string ToString()
            => $"--[{Id.ToString().Truncate(5)}: {Title.Truncate(20)}]-->";


        public bool Equals(GraphEdge other)
            => Id == other.Id;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals(obj as GraphEdge);
        }

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}