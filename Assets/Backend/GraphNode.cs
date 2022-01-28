using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using static Backend.StringExtensions;

namespace Backend
{
    public class GraphNode : IEquatable<GraphNode>
    {
        public string Title { get; private set; }
        public string Body { get; private set; }
        public Guid Id { get; private set; }

        public (double X, double Y, double Z) Coordinates { get; private set; }

        public IReadOnlyList<GraphEdge> Edges 
        {
            get => _edges.AsReadOnly();
        }

        public bool IsIsolated 
        {
            get => _edges.Count == 0;
        }

        private List<GraphEdge> _edges = new List<GraphEdge>();

        public GraphNode(string title, string body, Guid id, (double x, double y, double z) coordinates)
        {
            Title = title;
            Body = body;
            Id = id;
            Coordinates = coordinates;
        }

        public GraphNode(string title, string body, (double x, double y, double z) coordinates) :
            this(title, body, new Guid(), coordinates)
        { }

        public static GraphNode FromINode(INode dbNode)
        {
            string title = dbNode.Properties["title"].As<string>();
            string body = dbNode.Properties["body"].As<string>();
            string guidText = dbNode.Properties["guid"].As<string>();
            Guid guid = Guid.Parse(guidText);
            List<double> coords = dbNode.Properties["coordinates"].As<List<double>>();

            return new GraphNode(title, body, guid, (coords[0], coords[1], coords[2]));
        }

        public void AddEdge(GraphEdge edge)
            => _edges.Add(edge);
           
        public void RemoveEdge(GraphEdge edge)
            => _edges.Remove(edge);
        

        public override string ToString()
            => $"({Id.ToString().Truncate(5)}: {Title.Truncate(20)})";

        public bool Equals(GraphNode other)
            => Id == other.Id;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            
            return Equals(obj as GraphNode);
        }
        
        public override int GetHashCode()
            => Id.GetHashCode();
    }
}