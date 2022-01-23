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

        // TODO: make this sensible
        public List<GraphNode> ReadStartingNodes()
            => new List<GraphNode> 
            {
                ReadNodeWithGuid(Guid.Parse("20d39f6b-8662-4328-8dc5-df57eb3c4a3a")),
                ReadNodeWithGuid(Guid.Parse("199640fc-3605-4368-827c-a4e66551c0b5"))
            };

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

            var query_parent_child = $"MATCH (root {{guid: '{root.Id}'}}) -[edge :LINK]-> (child) RETURN edge, child";
            var query_child_parent = $"MATCH (root {{guid: '{root.Id}'}}) <-[edge :LINK]- (parent) RETURN edge, parent";

            using var session = _driver.Session();


            List<GraphEdge> edges = session.ReadTransaction(tx => 
            {
                var result_parent_child = tx.Run(query_parent_child); // Find all parent->child relationships from node
                var result_child_parent = tx.Run(query_child_parent);
                List<GraphEdge> edges = new List<GraphEdge>();

                foreach (var record in result_parent_child)
                {
                    INode childRead = record["child"].As<INode>();
                    GraphNode child = GraphNode.FromINode(childRead);

                    IRelationship edgeRead = record["edge"].As<IRelationship>();
                    GraphEdge edge = GraphEdge.FromIRelationship(edgeRead, root, child);
                    edges.Add(edge);
                }

                foreach (var record in result_child_parent)
                {
                    INode parentRead = record["parent"].As<INode>(); 
                    GraphNode parent = GraphNode.FromINode(parentRead);

                    IRelationship edgeRead = record["edge"].As<IRelationship>();
                    GraphEdge edge = GraphEdge.FromIRelationship(edgeRead, parent, root);
                    edges.Add(edge);
                }
                
                return edges;
            });

            foreach (GraphEdge edge in edges)
            {
                root.AddEdge(edge);
                AddAllNodesLinkedToRoot(edge.GetAttachedNode(root), nodesVisited);
            }
        }
    }
}
