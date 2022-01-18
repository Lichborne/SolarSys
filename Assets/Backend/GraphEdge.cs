using System;

namespace Backend
{
    public class GraphEdge
    {
        public string Text { get; private set; }
        public Guid Id { get; private set; }
        public GraphNode From { get; private set; }
        public GraphNode To { get; private set; }

        public GraphEdge(string text, Guid id, GraphNode from, GraphNode to)
        {
            Text = text;
            Id = id;
            From = from;
            To = to;
        }

        public GraphEdge(string text, GraphNode from, GraphNode to) :
            this(text, new Guid(), from, to)
        { }
    }
}