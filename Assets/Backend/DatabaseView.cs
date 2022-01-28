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
        public void CreateUnlinkedNode(GraphProject project, GraphNode node)
        {
            string query = $" MATCH (:USER {{email: '{project.UserEmail}'}}) " +
                            $" -[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{project.ProjectTitle}'}}) " +
                            $" CREATE (project_root) -[:CONTAINS]-> " + 
                            $" (:NODE {{guid: '{node.Id}', title: '{node.Title}', body: '{node.Body}', coordinates: [{node.Coordinates.X}, {node.Coordinates.Y}, {node.Coordinates.Z}]}})";
                
            using (var session = _driver.Session())
            {
                session.WriteTransaction(tx => tx.Run(query).Consume());
            }
        }


        /// <summary> Creates a parent-child edge bewteen the already-existing parent and child nodes that are contained in the same project root. </summary>
        public void CreateParentChildRelationship(GraphNode parent, GraphEdge edge, GraphNode child)
        {
            string query = $" MATCH (project_root :PROJECT_ROOT) -[:CONTAINS]-> (parent :NODE {{guid: '{parent.Id}'}}), " + 
                            $" (project_root) -[:CONTAINS]-> (child :NODE {{guid: '{child.Id}'}}) " + 
                            $" CREATE (parent) -[:LINK {{guid: '{edge.Id}', title: '{edge.Title}', body: '{edge.Body}'}}]-> (child)";
            
            Console.WriteLine($"trying query:\n{query}");
            
            using (var session = _driver.Session())
            {
                session.WriteTransaction(tx => tx.Run(query).Consume());
            }
        }


// ===================================== READ
        /// <summary> Returns a list of unlinked nodes from project with title `projectTitle`, owned by user with email `userEmail` </summary>
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

        /// <summary> Returns a list of all parent -> child edges from `allNodes`. Does not link nodes passed in. </summary>
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

// =========================== UPDATE

        /// <summary> Updates database by making sure that the node with GUID `nodeWithChanges.Id` has the same fields as `nodeWithChanges`. 
        /// Will not change the edges or children of `nodeWithChanges`.
        /// If no node with GUID `nodeWithChanges.Id` is found, no changes will happen. </summary>
        public void UpdateNodeFields(GraphNode nodeWithChanges)
        {
            string query = $"MATCH (node :NODE {{guid: '{nodeWithChanges.Id}'}}) " +
                $" SET node.title = '{nodeWithChanges.Title}', " + 
                $" node.body = '{nodeWithChanges.Body}', " +
                $" node.coordinates = [{nodeWithChanges.Coordinates.X}, {nodeWithChanges.Coordinates.Y}, {nodeWithChanges.Coordinates.Z}]";
            
            using (var session = _driver.Session())
            {
                session.WriteTransaction(tx => tx.Run(query).Consume());
            }
        }  

        /// <summary> Updates database by making sure that the edge with GUID `edgeWithChanges.Id` has the same fields as `edgeWithChanges`. 
        /// Will not change the parent or child of `edgeWithChanges`. 
        /// If no node with GUID `edgeWithChanges.Id` is found, no changes will happen. </summary>
        public void UpdateEdgeFields(GraphEdge edgeWithChanges)
        {
            string query = $"MATCH (:NODE {{guid: '{edgeWithChanges.Parent.Id}'}}) " +
                $"-[edge :LINK {{guid: '{edgeWithChanges.Id}'}}]-> " + 
                $" (:NODE {{guid: '{edgeWithChanges.Child.Id}'}})" +
                $" SET edge.title = '{edgeWithChanges.Title}', " +
                $" edge.body = '{edgeWithChanges.Body}' ";
            
            using (var session = _driver.Session())
            {
                session.WriteTransaction(tx => tx.Run(query).Consume());
            }
        }


// ==================== DESTROY
        /// <summary> Destroys the supplied node, along with all edges from which the node is either a parent or child.
        public void DestroyNode(GraphNode node)
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}', title: '{node.Title}', body: '{node.Body}'}}) " + 
                $"DETACH DELETE (node)";
            
            using (var session = _driver.Session())
            {
                session.WriteTransaction(tx => tx.Run(query).Consume());
            }
        }
    }
}
