using System;
using Neo4j.Driver;
using static Backend.StringExtensions;

namespace Backend
{
    public class GraphEdge
    {
        public string Text { get; private set; }
        public Guid Id { get; private set; }
        public GraphNode Parent { get; private set; }
        public GraphNode Child { get; private set; }

        public GraphEdge(string text, Guid id, GraphNode parent, GraphNode child)
        {
            Text = text;
            Id = id;
            Parent = parent;
            Child = child;
        }

        public GraphEdge(string text, GraphNode parent, GraphNode child) :
            this(text, Guid.NewGuid(), parent, child)
        { }

        public static GraphEdge FromIRelationship(IRelationship dbRelationship, GraphNode parent, GraphNode child)
        {
            string text = dbRelationship.Properties["text"].As<string>();
            string guidText = dbRelationship.Properties["guid"].As<string>();
            return new GraphEdge(text, Guid.Parse(guidText), parent, child);
        }

        public override string ToString()
            => $"--[{Id.ToString().Substring(0, 5)}: {Text.Truncate(20)}]-->";
    }
}