using System;
using System.Collections.Generic;

namespace Backend
{
    public class GraphNode
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


        public void AddEdge(GraphEdge edge)
            => _edges.Add(edge);
           
        public void RemoveEdge(GraphEdge edge)
            => _edges.Remove(edge);
    }
}