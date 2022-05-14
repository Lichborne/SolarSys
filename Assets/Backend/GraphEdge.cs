using System;
using static Backend.StringExtensions;
using System.Collections;

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
            if (parent == null)
                throw new NullReferenceException("GraphEdge received a null parent");

            if (child == null)
                throw new NullReferenceException("GraphEdge received a null child");

            if (parent.Project == null)
                throw new NullReferenceException("GraphEdge received a parent with a null project");

            Title = title?.Replace('"', '“');
            Description = body?.Replace('"', '“');
            Id = id;
            Parent = parent;
            Child = child;
            Project = parent?.Project;
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

        // public static GraphEdge FromIRelationship(IRelationship dbRelationship, GraphNode parent, GraphNode child)
        // {
        //     string title = dbRelationship.Properties["title"].As<string>();
        //     string description = dbRelationship.Properties["description"].As<string>();
        //     string guidText = dbRelationship.Properties["guid"].As<string>();
        //     return new GraphEdge(Guid.Parse(guidText), title, description, parent, child);
        // }

        public static GraphEdge FromJObject(JObject obj, GraphNode parent, GraphNode child)
        {
            string title = (string)obj["title"];
            string description = (string)obj["description"];
            string guidText = (string)obj["guid"];
            return new GraphEdge(Guid.Parse(guidText), title, description, parent, child);
        }

        // Writes the "parent -[edge] -> child" relationship stored in this edge to the database
        public IEnumerator CreateInDatabase()
            => Project.User.Database.CreateParentChildRelationshipCo(Parent, this, Child);

        public IEnumerator CreateInDatabaseCo()
        {
            yield return Project.User.Database.CreateParentChildRelationshipCo(Parent, this, Child);
        }

        // Updates the title of the edge, writing change to database
        public IEnumerator UpdateTitleCo(string title)
        {
            Title = title;
            yield return Project.User.Database.UpdateEdgeTitleCo(this, title);
        }

        // updates the description of the edge, writing change to database

        public IEnumerator UpdateDescriptionCo(string description)
        {
            Description = description;
            yield return Project.User.Database.UpdateEdgeDescriptionCo(this, description);
        }

        // deletes the edge from the database. does not affect the edge's parent or child
        public IEnumerator DeleteFromDatabaseCo(Action cleanupFunc = null) // works
        {
            yield return Project.User.Database.DestroyEdgeCo(this);
            if (cleanupFunc != null)
                cleanupFunc();
        }

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

        public String Serialize()
        {
            return $"{Title};{Description};{Id.ToString()};{Parent.Id};{Child.Id}";
        }
    }
}