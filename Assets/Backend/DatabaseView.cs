using Neo4j.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Backend
{
    public class DatabaseView : IDisposable
    {
        private bool _disposed = false;
        private IDriver _driver;
        private DatabaseConnection connection = new DatabaseConnection();

        ~DatabaseView()
            => Dispose(false);

        public DatabaseView(string uri, string username, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));
        }

        // the constructor for when we dont want to use Neo4J drivers -- this will replace the old constructor
        public DatabaseView()
        { }

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


        private void MakeAndLogChange(GraphProject project, string changeQuery, LogNode logNode)
        {
            Console.WriteLine($"Making change {changeQuery}");
            Guid headLogNodeId = GetHeadLogNodeId(project);

            if (headLogNodeId == Guid.Empty)
            {
                WriteQueries(
                    CreateUnlinkedLogNodeQuery(project, logNode),
                    changeQuery
                );
            }
            else
            {
                WriteQueries(
                    DestroyLogHistoryEdgeQuery(project),
                    CreateUnlinkedLogNodeQuery(project, logNode),
                    CreateLogLinkQuery(logNode.Id, headLogNodeId),
                    changeQuery
                );
            }
        }

        // Feb 15 2022: Don't create log nodes for the time being 
        // GetHeadLogNodeId calls the database and needs to be a coroutine
        // We cannot start a coroutine on unity here, as we need to be in a class
        // that extends from monobehaviour

        private IEnumerator MakeAndLogChangeQueryCo(GraphProject project, string changeQuery, LogNode logNode)
        {
            Debug.Log($"Making change {changeQuery}");
            Guid headLogNodeId = Guid.Empty;
            yield return GetHeadLogNodeIdCo(project, t => headLogNodeId = t);
           
            List<String> queries = new List<String>();

            if (headLogNodeId == Guid.Empty)
            {
                queries.Add(CreateUnlinkedLogNodeQuery(project, logNode));
                queries.Add(changeQuery);
            }
            else
            {
                queries.Add(DestroyLogHistoryEdgeQuery(project));
                queries.Add(CreateUnlinkedLogNodeQuery(project, logNode));
                queries.Add(CreateLogLinkQuery(logNode.Id, headLogNodeId));
                queries.Add(changeQuery);
            }
            yield return connection.SendWriteTransactions(queries);
        }

        // ===================================== Create
        /// <summary> Writes the node to the database, without any links </summary>
        public void CreateBlankGraphProject(GraphProject project)
        {
            string query = $"MATCH (user :USER {{email: '{project.ProjectId.UserEmail}'}})" + 
                $"MERGE (user) -[:OWNS_PROJECT]-> " + 
                $"(project_root :PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}})";
            
            connection.SendWriteTransactions(query);
        }

        // TODO: Replace CREATE with MERGE!
        public void CreateUnlinkedNode(GraphNode node)
        {
            string query = $" MATCH (:USER {{email: '{node.Project.ProjectId.UserEmail}'}}) " +
                            $" -[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{node.Project.ProjectId.ProjectTitle}'}}) " +
                            $" CREATE (project_root) -[:CONTAINS]-> " +
                            $" (:NODE {{guid: '{node.Id}', title: '{node.Title}', description: '{node.Description}', coordinates: [{node.Coordinates.X}, {node.Coordinates.Y}, {node.Coordinates.Z}]}})";


            LogNode logNode = new LogNode(ChangeEnum.Create, "json goes here");
            MakeAndLogChange(node.Project, query, logNode);
        }

        public IEnumerator CreateUnlinkedNodeCo(GraphNode node)
        {
            string query = $" MATCH (:USER {{email: '{node.Project.ProjectId.UserEmail}'}}) " +
                            $" -[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{node.Project.ProjectId.ProjectTitle}'}}) " +
                            $" CREATE (project_root) -[:CONTAINS]-> " +
                            $" (:NODE {{guid: '{node.Id}', title: '{node.Title}', description: '{node.Description}', coordinates: [{node.Coordinates.X}, {node.Coordinates.Y}, {node.Coordinates.Z}]}})";


            LogNode logNode = new LogNode(ChangeEnum.Create, "json goes here");
            // string[] queries = MakeAndLogChangeQuery(node.Project, query, logNode).ToArray();
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }

        /// <summary> Writes to the database a log node that is linked to a project, but not linked to other log nodes </summary>
        private static string CreateUnlinkedLogNodeQuery(GraphProject project, LogNode node)
            => $" MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                            $" -[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                            $" CREATE (project_root) -[:LOG_HISTORY]-> " +
                            $" (:LOG_NODE {{guid: '{node.Id}', change: '{node.Change}', body: '{node.Body}', timestamp: '{node.TimeStamp}'}})";

        public void CreateUnlinkedLogNode(GraphProject project, LogNode node)
        {
            string query = CreateUnlinkedLogNodeQuery(project, node);
            WriteQuery(query);
        }

        /// <summary> Add new log node to chain of log nodes </summary>
        public void AppendLogNode(GraphProject project, LogNode node)
        {
            // identify current log head, and remove its links to project node

            var guidHead = GetHeadLogNodeId(project);

            if (!guidHead.Equals(Guid.Empty))
            {
                DestroyLogHistoryEdge(project);

                // make node the new log head 
                CreateUnlinkedLogNode(project, node);

                // attach new log head node to previous log head node
                CreateLogLink(node.Id, guidHead);
            }
            else
            {
                // make node the new log head 
                CreateUnlinkedLogNode(project, node);
            }
        }

        /// <summary> Creates a parent-child edge bewteen the already-existing parent and child nodes that are contained in the same project root. </summary>
        public void CreateParentChildRelationship(GraphNode parent, GraphEdge edge, GraphNode child)
        {
            string query = $" MATCH (project_root :PROJECT_ROOT) -[:CONTAINS]-> (parent :NODE {{guid: '{parent.Id}'}}), " +
                            $" (project_root) -[:CONTAINS]-> (child :NODE {{guid: '{child.Id}'}}) " +
                            $" CREATE (parent) -[:LINK {{guid: '{edge.Id}', title: '{edge.Title}', description: '{edge.Description}'}}]-> (child)";

            LogNode logNode = new LogNode(ChangeEnum.Create, "json goes here");
            MakeAndLogChange(parent.Project, query, logNode);
        }

        public IEnumerator CreateParentChildRelationshipCo(GraphNode parent, GraphEdge edge, GraphNode child) // works
        {
            string query = $" MATCH (project_root :PROJECT_ROOT) -[:CONTAINS]-> (parent :NODE {{guid: '{parent.Id}'}}), " +
                $" (project_root) -[:CONTAINS]-> (child :NODE {{guid: '{child.Id}'}}) " +
                $" CREATE (parent) -[:LINK {{guid: '{edge.Id}', title: '{edge.Title}', body: '{edge.Description}'}}]-> (child)";

            LogNode logNode = new LogNode(ChangeEnum.Create, "json goes here");
            yield return MakeAndLogChangeQueryCo(parent.Project, query, logNode);

        }
        /// <summary> Creates a parent-child edge bewteen the already-existing parent and child log nodes that are contained in the same project root. </summary>
        private static string CreateLogLinkQuery(Guid parentId, Guid childId)
            => $" MATCH (parent :LOG_NODE {{guid: '{parentId}'}}), " +
                            $" (child :LOG_NODE {{guid: '{childId}'}}) " +
                            " CREATE (parent) -[:LOG_LINK]-> (child)";

        public void CreateLogLink(Guid parentId, Guid childId)
        {
            string query = CreateLogLinkQuery(parentId, childId);
            WriteQuery(query); 
        }

        public IEnumerator CreateBlankPathRoot(GraphProject project, PathRoot path)
        {
            string query = $" MATCH (user :USER {{email: '{project.ProjectId.UserEmail}'}}) " + 
                $"-[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " + 
                $"MERGE (project_root) -[:HAS_PATH]-> " + 
                $" (path_root :PATH_ROOT {{guid: '{path.Id}', title: '{path.Title}', description: '{path.Description}'}})";
            
            yield return connection.SendWriteTransactions(query);
        }

        // ===================================== READ
        /// <summary> Returns a list of unlinked nodes from project with title `projectTitle`, owned by user with email `userEmail` </summary>
        public List<GraphNode> ReadNodesFromProject(GraphProject project)
        {
            string query = $"MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                $" -[:CONTAINS]->(node :NODE) " +
                $" RETURN node";

            using (var session = _driver.Session())
            {
                return session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);
                    List<GraphNode> nodes = new List<GraphNode>();

                    foreach (var record in result)
                        nodes.Add(GraphNode.FromINode(project, record["node"].As<INode>()));

                    return nodes;
                });
            }
        }

        public IEnumerator ReadNodesFromProjectCo(GraphProject project, Action<List<GraphNode>> processGraphNodes) // works
        {
            string query = $"MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                $" -[:CONTAINS]->(node :NODE) " +
                $" RETURN node";

            // Debug.Log($"ReadNodesFromProjectCo() Running query \n{query}");
            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, t => table = t);
            List<GraphNode> nodes = new List<GraphNode>();
            foreach (Dictionary<string, JToken> row in table)
            {
                JObject node = row["node"] as JObject;
                nodes.Add(GraphNode.FromJObject(project, node));
            }
            processGraphNodes(nodes);
        }

        /// <summary> Returns a list of log nodes linked to `projectTitle`, owned by user with email `userEmail` </summary>
        public List<LogNode> ReadLogNodesFromProject(GraphProject project)
        {
            string query = $"MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                $" -[:LOG_HISTORY]->(node :LOG_NODE) " +
                $" RETURN node";

            using (var session = _driver.Session())
            {
                return session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);
                    List<LogNode> nodes = new List<LogNode>();

                    foreach (var record in result)
                        nodes.Add(LogNode.FromINode(record["node"].As<INode>()));

                    return nodes;
                });
            }
        }

        public IEnumerator ReadLogNodesFromProjectCo(GraphProject project, Action<List<LogNode>> processLogNodes) // works
        {
            string query = $"MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                $" -[*]->(node :LOG_NODE) " +
                $" RETURN node";

            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, t => table = t);

            List<LogNode> nodes = new List<LogNode>();
            foreach (Dictionary<string, JToken> row in table)
            {
                JObject node = row["node"] as JObject;
                nodes.Add(LogNode.FromJObject(project, node));
            }
            processLogNodes(nodes);

        }

        /// <summary> Returns a list of graph nodes representing project titles that linked to the user Email  </summary>

        public List<String> ReadAllProjectTitlesAttachedToUser(String userEmail)
        {
            string query = $"MATCH (:USER {{email: '{userEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (project:PROJECT_ROOT) " +
                $" RETURN project";

            using (var session = _driver.Session())
            {
                return session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);
                    List<String> projectTitles = new List<String>();

                    foreach (var record in result)
                        projectTitles.Add(record["project"].As<INode>().Properties["title"].As<string>());
                    return projectTitles;
                });
            }
        }

        public IEnumerator ReadAllProjectTitlesAttachedToUserCo(string userEmail, Action<List<string>> processTitles) // Works!
        {
            string query = $"MATCH (:USER {{email: '{userEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (project:PROJECT_ROOT) " +
                $" RETURN project";

            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, t => table = t);

            List<string> projectTitles = new List<string>();
            foreach (Dictionary<string, JToken> row in table)
            {
                JObject project = row["project"] as JObject;
                string title = (string)project["title"];
                projectTitles.Add(title);
            }

            yield return "waiting for next frame :) the next function might take a while to run lol";
            processTitles(projectTitles);
        }


        public Guid GetHeadLogNodeId(GraphProject project)
        {
            string query = $"MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                            $"-[:OWNS_PROJECT]->(:PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                            "-[:LOG_HISTORY]-> (node) RETURN node";

            using (var session = _driver.Session())
            {
                return session.ReadTransaction(tx =>
                {
                    var result = tx.Run(query);

                    try
                    {
                        String guidString = result.Single()["node"].As<INode>().Properties["guid"].As<string>();
                        return Guid.Parse(guidString);
                    }
                    catch (System.InvalidOperationException)
                    {
                        // No results in query
                        return Guid.Empty;
                    }

                });
            }
        }

        public IEnumerator GetHeadLogNodeIdCo(GraphProject project, Action<Guid> processGuid)
        {
            string query = $"MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                            $"-[:OWNS_PROJECT]->(:PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                            "-[:LOG_HISTORY]-> (node) RETURN node";

            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, t => table = t);
            if (table.Any())
            {
                JObject headLogNode = table.First()["node"] as JObject;
                var guidString = (string)headLogNode["guid"];
                processGuid(Guid.Parse(guidString));
            }
            else
            {
                var guidEmpty = Guid.Empty;
                processGuid(guidEmpty);
            }

        }

        /// <summary> Returns a list of all parent -> child edges from `allNodes`. Does not link nodes passed in. </summary>
        public List<GraphEdge> ReadAllEdgesFromProject((string userEmail, string projectTitle) projectId, List<GraphNode> allNodes)
        {
            string query = $"MATCH (:USER {{email: '{projectId.userEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{projectId.projectTitle}'}}) " +
                $" -[:CONTAINS]->(parent :NODE) -[edge :LINK]-> (child :NODE) " +
                $" RETURN parent, edge, child";
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

        public IEnumerator ReadGraphNodesInPath(PathRoot path, Action<List<GraphNode>> processGraphNodes)
        {
            string query = $"MATCH (path_root :PATH_ROOT {{guid: '{path.Id}'}})" +
                $"-[:VIEWS]-> (node :NODE)" + 
                $"RETURN node";
            
            List<GraphNode> graphNodes = new List<GraphNode>();
            yield return connection.SendReadTransaction(query, table => 
            {
                foreach (Dictionary<string, JToken> row in table)
                {
                    JObject json = row["node"] as JObject;
                    graphNodes.Add(GraphNode.FromJObject(path.Project, json));
                }
            });

            yield return "waiting for next frame :)";
            processGraphNodes(graphNodes);
        }
        
        public IEnumerator ReadAllEdgesFromProjectCo(GraphProject project, List<GraphNode> allNodes, Action<List<GraphEdge>> processGraphEdges) // works
        {
            string query = $"MATCH (:USER {{email: '{project.ProjectId.UserEmail}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.ProjectId.ProjectTitle}'}}) " +
                $" -[:CONTAINS]->(parent :NODE) -[edge :LINK]-> (child :NODE) " +
                $" RETURN parent, edge, child";

            List<GraphEdge> edges = new List<GraphEdge>();

            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, t => table = t);

            foreach (Dictionary<string, JToken> row in table)
            {
                JObject parentObj = row["parent"] as JObject;
                Guid parentId = Guid.Parse((string)parentObj["guid"]);

                JObject childObj = row["child"] as JObject;
                Guid childId = Guid.Parse((string)childObj["guid"]);


                // Find parent and child graph node object from list of all nodes
                GraphNode parentNode = allNodes.Find(node => node.Id == parentId);
                if (parentNode == null)
                    throw new Exception($"Could not find parent node with id = {parentId}");

                GraphNode childNode = allNodes.Find(node => node.Id == childId);
                if (childNode == null)
                    throw new Exception($"Could not find child with id = {childId}");


                // Create GraphEdge object
                JObject edgeObj = row["edge"] as JObject;

                GraphEdge edge = GraphEdge.FromJObject(edgeObj, parentNode, childNode);

                edges.Add(edge);

            }
            processGraphEdges(edges);
        }

        // =========================== UPDATE

        /// <summary> Updates database by making sure that the node with GUID `nodeWithChanges.Id` has the same fields as `nodeWithChanges`. 
        /// Will not change the edges or children of `nodeWithChanges`.
        /// If no node with GUID `nodeWithChanges.Id` is found, no changes will happen. </summary>
        /*public void UpdateNodeFields(GraphNode nodeWithChanges)
        {
            string query = $"MATCH (node :NODE {{guid: '{nodeWithChanges.Id}'}}) " +
                $" SET node.title = '{nodeWithChanges.Title}', " +
                $" node.body = '{nodeWithChanges.Description}', " +
                $" node.coordinates = [{nodeWithChanges.Coordinates.X}, {nodeWithChanges.Coordinates.Y}, {nodeWithChanges.Coordinates.Z}]";

            WriteQuery(query);
        } */

        public void UpdateNodeTitle(GraphNode node, string title)
        {
            string updateTitleQuery = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.title = '{title}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            MakeAndLogChange(node.Project, updateTitleQuery, logNode);
        }

        public IEnumerator UpdateNodeTitleCo(GraphNode node, string title) // Works!
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.title = '{title}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }

        public void UpdateNodeDescription(GraphNode node, string description)
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.description = '{description}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            MakeAndLogChange(node.Project, query, logNode);
        }

        public IEnumerator UpdateNodeDescriptionCo(GraphNode node, string description) // Works!
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.description = '{description}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }

        public void UpdateNodeCoordinates(GraphNode node, (double x, double y, double z) coordinates)
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.coordinates = [{coordinates.x}, {coordinates.y}, {coordinates.z}]";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            MakeAndLogChange(node.Project, query, logNode);
        }

        public IEnumerator UpdateNodeCoordinatesCo(GraphNode node, (double x, double y, double z) coordinates) // Works!
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.coordinates = [{coordinates.x}, {coordinates.y}, {coordinates.z}]";

            // Debug.Log("query = " + query);
            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }

        /// <summary> Updates database by making sure that the edge with GUID `edgeWithChanges.Id` has the same fields as `edgeWithChanges`. 
        /// Will not change the parent or child of `edgeWithChanges`. 
        /// If no node with GUID `edgeWithChanges.Id` is found, no changes will happen. </summary>

        public void UpdateEdgeTitle(GraphEdge edge, string title)
        {
            string updateTitleQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $"-[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}})" +
                $" SET edge.title = '{title}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            MakeAndLogChange(edge.Project, updateTitleQuery, logNode);
        }

        public IEnumerator UpdateEdgeTitleCo(GraphEdge edge, string title)
        {
            string updateTitleQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $"-[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}})" +
                $" SET edge.title = '{title}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            yield return MakeAndLogChangeQueryCo(edge.Project, updateTitleQuery, logNode);
        }

        public void UpdateEdgeDescription(GraphEdge edge, string description)
        {
            string updateDescQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $"-[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}})" +
                $" SET edge.description = '{description}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            MakeAndLogChange(edge.Project, updateDescQuery, logNode);
        }

        public IEnumerator AddNodeToPath(PathRoot path, GraphNode node)
        {
            string query = $"MATCH (path_root :PATH_ROOT {{guid: '{path.Id}'}})," + 
                $"(node :NODE {{guid: '{node.Id}'}})" +
                $"MERGE (path_root) -[:VIEWS]-> (node)";
            
            yield return connection.SendWriteTransactions(query);
        }

        public IEnumerator UpdateEdgeDescriptionCo(GraphEdge edge, string description)
        {
            string updateDescQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $"-[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}})" +
                $" SET edge.description = '{description}'";

            LogNode logNode = new LogNode(ChangeEnum.Update, "json goes here");
            yield return MakeAndLogChangeQueryCo(edge.Project, updateDescQuery, logNode);
        }

        // ==================== DESTROY
        /// <summary> Destroys the supplied node, along with all edges from which the node is either a parent or child.
        public void DestroyNode(GraphNode node)
        {
            string deleteQuery = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $"DETACH DELETE (node)";

            LogNode logNode = new LogNode(ChangeEnum.Delete, "json goes here");
            MakeAndLogChange(node.Project, deleteQuery, logNode);
        }

        public IEnumerator DestroyNodeCo(GraphNode node) // works
        {
            string deleteQuery = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $"DETACH DELETE (node)";

            LogNode logNode = new LogNode(ChangeEnum.Delete, "json goes here");
            yield return MakeAndLogChangeQueryCo(node.Project, deleteQuery, logNode);
        }
        public void DestroyEdge(GraphEdge edge)
        {
            string deleteEdgeQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $" -[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}}) " +
                $" DELETE edge";

            LogNode logNode = new LogNode(ChangeEnum.Delete, "json goes here");
            MakeAndLogChange(edge.Project, deleteEdgeQuery, logNode);
        }

        public IEnumerator DestroyEdgeCo(GraphEdge edge)
        {
            string deleteEdgeQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $" -[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}}) " +
                $" DELETE edge";

            LogNode logNode = new LogNode(ChangeEnum.Delete, "json goes here");
            yield return MakeAndLogChangeQueryCo(edge.Project, deleteEdgeQuery, logNode);
        }

        private static string DestroyLogHistoryEdgeQuery(GraphProject project)
            => $"MATCH ({{title: '{project.ProjectId.ProjectTitle}'}}) " +
                " -[r:LOG_HISTORY]->(n) " +
                " DELETE r";

        public void DestroyLogHistoryEdge(GraphProject project)
        {
            string query = DestroyLogHistoryEdgeQuery(project);
            WriteQuery(query);
        }

        public IEnumerator RemoveNodeFromPath(PathRoot path, GraphNode node)
        {
            string query = $"MATCH (path_root :PATH_ROOT {{guid: '{path.Id}'}}" + 
                $"-[edge :VIEWS]->" + 
                $"(node :NODE {{guid: '{node.Id}'}})" + 
                $"DELETE edge";
            
            yield return connection.SendWriteTransactions(query);
        }

        public IEnumerator DeletePath(PathRoot path)
        {
            string query = $"MATCH (path_root :PATH_ROOT {{guid: '{path.Id}'}}" + 
                $"DETACH DELETE path_root";
            
            yield return connection.SendWriteTransactions(query);
        }

        private void WriteQuery(string query)
        {
            using (var session = _driver.Session())
            {
                session.WriteTransaction(tx => tx.Run(query).Consume());
            }
        }

        private void WriteQueries(params string[] queries)
        {
            using (var session = _driver.Session())
            {
                session.WriteTransaction(tx =>
                {
                    IResultSummary summary = null;
                    foreach (string query in queries)
                        summary = tx.Run(query).Consume();

                    return summary;
                });
            }
        }
    }
}
