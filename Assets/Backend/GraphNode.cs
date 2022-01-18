using System;
using System.Collections.Generic;

namespace Backend
{
    class GraphNode
    {
        public string Text { get; private set; }
        public Guid Id { get; private set; }

        public IReadOnlyList<GraphEdge> Edges 
        {
            get => _edges.AsReadOnly();
        }

        public bool IsIsolated 
        {
            get => _edges.Count() == 0;
        }

        private List<GraphEdge> _edges { get; private set; } = new List<GraphEdge>();

        public GraphNode(string text, Guid id)
        {
            Text = text;
            Id = id;
        }

        public GraphNode(string text)
            => GraphNode(text, new Guid());


        public void AddEdge(GraphEdge edge)
            => _edges.Add(edge);
           
        public void RemoveEdge(GraphEdge edge)
            => _edges.Remove(edge);
    }
}