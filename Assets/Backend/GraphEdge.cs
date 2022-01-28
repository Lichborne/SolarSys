using System;
using Neo4j.Driver;
using static Backend.StringExtensions;

namespace Backend
{
    public class GraphEdge : IEquatable<GraphEdge>
    {
        public string Title { get; private set; }
        public string Body {get; private set; }

        public Guid Id { get; private set; }
        public GraphNode Parent { get; private set; }
        public GraphNode Child { get; private set; }

        public GraphEdge(Guid id, string title, string body, GraphNode parent, GraphNode child)
        {
            Title = title;
            Body = body;
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
            else {
                throw new ArgumentException("Node not attached to edge");
            }
        }

        public static GraphEdge FromIRelationship(IRelationship dbRelationship, GraphNode parent, GraphNode child)
        {
            string title = dbRelationship.Properties["title"].As<string>();
            string body = dbRelationship.Properties["body"].As<string>();
            string guidText = dbRelationship.Properties["guid"].As<string>();
            return new GraphEdge(Guid.Parse(guidText), title, body, parent, child);
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
    }
}