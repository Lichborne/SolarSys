using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using static Backend.StringExtensions;

namespace Backend
{
    public class GraphNode : IEquatable<GraphNode>
    {
        public string Text { get; private set; }
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

        public GraphNode(string text, Guid id, (double x, double y, double z) coordinates)
        {
            Text = text;
            Id = id;
            Coordinates = coordinates;
        }

        public GraphNode(string text, (double x, double y, double z) coordinates) :
            this(text, new Guid(), coordinates)
        { }

        public static GraphNode FromINode(INode dbNode)
        {
            string text = dbNode.Properties["text"].As<string>();
            string guidText = dbNode.Properties["guid"].As<string>();
            Guid guid = Guid.Parse(guidText);
            List<double> coords = dbNode.Properties["coordinates"].As<List<double>>();

            return new GraphNode(text, guid, (coords[0], coords[1], coords[2]));
        }

        public override string ToString()
            => $"({Id.ToString().Truncate(5)}: {Text.Truncate(20)})";

        public void AddEdge(GraphEdge edge)
            => _edges.Add(edge);
           
        public void RemoveEdge(GraphEdge edge)
            => _edges.Remove(edge);
        
        public override bool Equals(GraphNode other)
            => Id == other.Id;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            
            
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new System.NotImplementedException();
            return base.GetHashCode();
        }
    }
}