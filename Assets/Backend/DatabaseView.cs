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

// ===================================== Create
        /// <summary> Writes the node to the database, without any links </summary>
        public void CreateUnlinkedNode(GraphNode node)
        {
            string query = $"MERGE ({{guid: {node.Id}, }})";
        }


// ===================================== READ
        /// <summary> From database, recursively reads in all nodes linked to root (either --> or <--). Gives each parent found an edge to all children. </summary>
        public List<GraphNode> ReadNodesFromProject(string userEmail, string projectTitle)
        {
            string query = $"MATCH (:USER {{email: '{userEmail}'}}) -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{projectTitle}'}}) -[:CONTAINS]->(node :NODE) RETURN node";
            using (var session = _driver.Session())
            {
                return session.ReadTransaction(tx => 
                {
                    var result = tx.Run(query);
                    List<GraphNode> nodes = new List<GraphNode>();

                    foreach (var record in result)
                        nodes.Add(GraphNode.FromINode(record["node"].As<INode>()));

                    return nodes;
                });
            }
        }


        public List<GraphEdge> ReadAllEdgesFromProject(string userEmail, string projectTitle, List<GraphNode> allNodes)
        {
            string query = $"MATCH (:USER {{email: '{userEmail}'}}) -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{projectTitle}'}}) -[:CONTAINS]->(parent :NODE) -[edge :LINK]-> (child :NODE) RETURN parent, edge, child";
            using var session = _driver.Session();

            return session.ReadTransaction(tx => 
            {
                var result = tx.Run(query);
                List<GraphEdge> edges = new List<GraphEdge>();
                foreach (var record in result)
                {
                    Guid parentId = Guid.Parse(record["parent"].As<INode>().Properties["guid"].As<string>());
                    Guid childId = Guid.Parse(record["child"].As<INode>().Properties["guid"].As<string>());
                    
                    GraphNode parent = allNodes.Find(node => node.Id == parentId);
                    if (parent == null)
                        throw new Exception($"Could not find parent node with id = {parentId}");
                    

                    GraphNode child = allNodes.Find(node => node.Id == childId);
                    if (child == null)
                        throw new Exception($"Could not find child with id = {childId}");
                    
                    GraphEdge edge = GraphEdge.FromIRelationship(record["edge"].As<IRelationship>(), parent, child);

                    edges.Add(edge);
                }
                return edges;
            });
        }


        /*
        public void ReadAllNodesLinkedToRoot(GraphNode root)
        {
            HashSet<GraphNode> nodesVisited = new HashSet<GraphNode>();
            ReadAllNodesLinkedToRoot(root, nodesVisited);
        }

        
        private void ReadAllNodesLinkedToRoot(GraphNode root, HashSet<GraphNode> nodesVisited)
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
                ReadAllNodesLinkedToRoot(edge.GetAttachedNode(root), nodesVisited);
            }
        } */
    }
}
