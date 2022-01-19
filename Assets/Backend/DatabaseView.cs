using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    public class DatabaseView : IDisposable
    {
        private bool _disposed = false;
        private IDriver _driver;

        ~DatabaseView()
            => Dispose(false);

        public DatabaseView(string uri, string username, string password)
            => _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));

        /// <summary> Closes the database connection. </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                _driver?.Dispose();

            _disposed = true;
        }

        public GraphNode ReadNodeWithGuid(Guid id)
        {
            var query = $"MATCH (node {{guid: '{id}'}}) RETURN node";
            using var session = _driver.Session();

            return session.ReadTransaction(tx => 
            {
                var result = tx.Run(query);
                try 
                {
                    var record = result.Single();
                    var node = record["node"].As<INode>();
                    return GraphNode.FromINode(node); 
                }   
                catch (InvalidOperationException)
                {
                    throw new Exception($"Error (probably found no nodes) in running query \n{query}");
                }
            });
        }

        /// <summary> From database, recursively reads in all nodes linked to root (either --> or <--). Gives each parent found an edge to all children. </summary>
        public void AddAllNodesLinkedToRoot(GraphNode root)
        {
            HashSet<GraphNode> nodesVisited = new HashSet<GraphNode>();
            AddAllNodesLinkedToRoot(root, nodesVisited);
        }


        private void AddAllNodesLinkedToRoot(GraphNode root, HashSet<GraphNode> nodesVisited)
        {
            if (nodesVisited.Contains(root))
                return;
            
            nodesVisited.Add(root);
            // in query, find all relations and children of root
            // add relations with children to root (without creating childrens relations)
            // recursive call on each child

            var query = $"MATCH (root {{guid: '{root.Id}'}}) -[edge :LINK]- (child) RETURN edge, child";
            using var session = _driver.Session();


            List<GraphEdge> edges = session.ReadTransaction(tx => 
            {
                var result = tx.Run(query);
                List<GraphEdge> edges = new List<GraphEdge>();

                foreach (var record in result)
                {
                    INode childRead = record["child"].As<INode>();
                    GraphNode child = GraphNode.FromINode(childRead);

                    IRelationship edgeRead = record["edge"].As<IRelationship>();
                    GraphEdge edge = GraphEdge.FromIRelationship(edgeRead, root, child);
                    edges.Add(edge);
                }
                
                return edges;
            });

            foreach (GraphEdge edge in edges)
            {
                root.AddEdge(edge);
                AddAllNodesLinkedToRoot(edge.Child, nodesVisited);
            }
        }
    }
}
