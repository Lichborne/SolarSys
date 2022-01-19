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

        /// <summary> IDs of the nodes that <paramref name="nodeId"/> links to. </summary>
        public List<int> NodeIdsLinkedFrom(int nodeId)
            => NodeIdsMatchingQuery($"match (linkingNode) --> (baseNode) where id(linkingNode) = {nodeId} return id(baseNode)");

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

        public void ReadAllChildrenFromRoot(GraphNode root)
        {
            HashSet<GraphNode> nodesVisited = new HashSet<GraphNode>();
            ReadAllChildrenFromRoot(root, nodesVisited);
        }


        private void ReadAllChildrenFromRoot(GraphNode root, HashSet<GraphNode> nodesVisited)
        {
            if (nodesVisited.Any(node => node.Id == root.Id))
                return;
            
            nodesVisited.Add(root);
            // in query, find all relations and children of root
            // add relations with children to root (without creating childrens relations)
            // recursive call on each child

            var query = $"MATCH (parent {{guid: '{root.Id}'}}) -[edge :LINK]-> (child) RETURN edge, child";
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
                    Console.WriteLine($"{root} {edge} {child}");
                    edges.Add(edge);
                }
                
                return edges;
            });

            foreach (GraphEdge edge in edges)
            {
                root.AddEdge(edge);
                ReadAllChildrenFromRoot(edge.Child, nodesVisited);
            }
        }

    /*
        public NodeTree CreateNodeTreeFromGuid(string guid)
        {
            var query = $"match (n {{'guid': {guid}}}) return n";
            using var session = _driver.Session();

            NodeTree nodeTree = session.ReadTransaction(tx =>
            {
                var result = tx.Run(query);
                var record = result.Single();
                var content = record["content"].As<string>();
                var coordinates = record["coordinates"].As<List<double>>();
                return new NodeTree(coordinates, content, guid);
            }).ToList();

            return nodeTree;
        } */

        /// <summary> IDs of the nodes that link to <paramref name="nodeId"/>. </summary>
        public List<int> NodeIdsLinkingTo(int nodeId)
            => NodeIdsMatchingQuery($"match (linkingNode) --> (baseNode) where id(baseNode) = {nodeId} return id(linkingNode)");

        /// <summary> The IDs of every node in the database. </summary>
        public List<int> AllNodeIds()
            => NodeIdsMatchingQuery("match (x) return id(x)");

        /// <summary> The IDs returned by <paramref name="query"/>. <paramref name="query"/> must return node IDs. </summary>
        private List<int> NodeIdsMatchingQuery(string query)
        {
            using var session = _driver.Session();

            List<int> nodeIds = session.ReadTransaction(tx =>
            {
                var result = tx.Run(query);
                List<int> ids = new List<int>();

                foreach (var record in result)
                    ids.Add(record[0].As<int>());

                return ids;
            }).ToList();

            return nodeIds;
        }  
    }
}
