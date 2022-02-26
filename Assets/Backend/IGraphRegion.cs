using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Backend 
{
    public interface IGraphRegion
    {
        public Guid Id { get; }

        public string Title { get; }

        public List<GraphNode> Nodes { get; }

        public List<GraphEdge> Edges { get; }

        public bool IsEmpty { get; }

        public IEnumerator CreateInDatabase();
    }
}