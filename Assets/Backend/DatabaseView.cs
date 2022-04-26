using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Backend
{
    public class DatabaseView
    {
        private DatabaseConnection connection;

        public DatabaseView(string username, string password)
        {
            connection = new DatabaseConnection(username: username, password: password);
        }

        public DatabaseView()
        {
            connection = new DatabaseConnection();
        }



        // ========================== CREATE
        private IEnumerator MakeAndLogChangeQueryCo(GraphProject project, string changeQuery, LogNode logNode)
        {
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

        public IEnumerator CreateBlankGraphProject(GraphProject project)
        {
            string query = $"MATCH (user :USER {{email: '{project.User.Email}'}})" +
                $"MERGE (user) -[:OWNS_PROJECT]-> " +
                $"(project_root :PROJECT_ROOT {{title: '{project.Title}', guid: '{project.Id}'}})";

            yield return connection.SendWriteTransactions(query);
        }

        public IEnumerator CreateUnlinkedNode(GraphNode node)
        {
            string query = $" MATCH (:USER {{email: '{node.Project.User.Email}'}}) " +
                            $" -[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{node.Project.Title}'}}) " +
                            $" MERGE (project_root) -[:CONTAINS]-> " +
                            $" (:NODE {{guid: '{node.Id}', title: '{node.Title}', description: '{node.Description}', coordinates: [{node.Coordinates.X}, {node.Coordinates.Y}, {node.Coordinates.Z}]}})";


            LogNode logNode = new LogNode(ChangeEnum.AddNode, "json goes here");
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }

        public IEnumerator CreateUnlinkedNodeCo(GraphNode node)
        {
            string query = $" MATCH (:USER {{email: '{node.Project.User.Email}'}}) " +
                            $" -[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{node.Project.Title}'}}) " +
                            $" CREATE (project_root) -[:CONTAINS]-> " +
                            $" (:NODE {{guid: '{node.Id}', title: '{node.Title}', description: '{node.Description}', coordinates: [{node.Coordinates.X}, {node.Coordinates.Y}, {node.Coordinates.Z}]}})";


            LogNode logNode = new NodeCreationLog(node);
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }

        private static string CreateUnlinkedLogNodeQuery(GraphProject project, LogNode node)
            => $" MATCH (:USER {{email: '{project.User.Email}'}}) " +
                            $" -[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{project.Title}'}}) " +
                            $" CREATE (project_root) -[:LOG_HISTORY]-> " +
                            $" (:LOG_NODE {{guid: '{node.Id}', change: '{node.Change}', body: '{node.Body}', timestamp: '{node.TimeStamp}'}})";


        public IEnumerator CreateParentChildRelationshipCo(GraphNode parent, GraphEdge edge, GraphNode child) // works
        {
            string query = $" MATCH (project_root :PROJECT_ROOT) -[:CONTAINS]-> (parent :NODE {{guid: '{parent.Id}'}}), " +
                $" (project_root) -[:CONTAINS]-> (child :NODE {{guid: '{child.Id}'}}) " +
                $" CREATE (parent) -[:LINK {{guid: '{edge.Id}', title: '{edge.Title}', body: '{edge.Description}'}}]-> (child)";

            LogNode logNode = new EdgeCreationLog(parent, edge, child);
            yield return MakeAndLogChangeQueryCo(parent.Project, query, logNode);

        }
        private static string CreateLogLinkQuery(Guid parentId, Guid childId)
            => $" MATCH (parent :LOG_NODE {{guid: '{parentId}'}}), " +
                            $" (child :LOG_NODE {{guid: '{childId}'}}) " +
                            " CREATE (parent) -[:LOG_LINK]-> (child)";


        public IEnumerator CreateBlankPathRoot(GraphProject project, PathRoot path)
        {
            string query = $" MATCH (user :USER {{email: '{project.User.Email}'}}) " +
                $"-[:OWNS_PROJECT]-> (project_root :PROJECT_ROOT {{title: '{project.Title}'}}) " +
                $"MERGE (project_root) -[:HAS_PATH]-> " +
                $" (path_root :PATH_ROOT {{guid: '{path.Id}', title: '{path.Title}', description: '{path.Description}'}})";

            yield return connection.SendWriteTransactions(query);
        }

        public IEnumerator CreateUserIfNotExists(string userEmail)
        {
            string query = $"MERGE (user :USER {{email: '{userEmail}'}})";
            yield return connection.SendWriteTransactions(query);
        }




        // =============================== READ
        public IEnumerator ReadNodesFromProjectCo(GraphProject project, Action<List<GraphNode>> processGraphNodes) // works
        {
            string query = $"MATCH (:USER {{email: '{project.User.Email}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.Title}'}}) " +
                $" -[:CONTAINS]->(node :NODE) " +
                $" RETURN node";

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

        public IEnumerator ReadLogNodesFromProjectCo(GraphProject project, Action<List<LogNode>> processLogNodes) // works
        {
            string query = $"MATCH (:USER {{email: '{project.User.Email}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.Title}'}}) " +
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

        public IEnumerator ReadAllEmptyProjects(GraphUser user, Action<List<GraphProject>> processGraphProjects) // Works!
        {
            string query = $"MATCH (:USER {{email: '{user.Email}'}}) " +
                $" -[:OWNS_PROJECT]-> (project:PROJECT_ROOT) " +
                $" RETURN project";

            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, t => table = t);

            List<GraphProject> projects = new List<GraphProject>();
            foreach (Dictionary<string, JToken> row in table)
            {
                JObject projectJson = row["project"] as JObject;
                projects.Add(GraphProject.FromJObject(user, projectJson));
            }

            yield return "waiting for next frame :) the next function might take a while to run lol";
            processGraphProjects(projects);
        }

        public IEnumerator GetHeadLogNodeIdCo(GraphProject project, Action<Guid> processGuid)
        {
            string query = $"MATCH (:USER {{email: '{project.User.Email}'}}) " +
                            $"-[:OWNS_PROJECT]->(:PROJECT_ROOT {{title: '{project.Title}'}}) " +
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


        public IEnumerator ReadNodeIdsInPath(PathRoot path, Action<List<Guid>> processNodeIds)
        {
            string query = $"MATCH (path_root :PATH_ROOT {{guid: '{path.Id}'}})" +
                $"-[:VIEWS]-> (node :NODE)" +
                $"RETURN node.guid as guid";

            List<Guid> nodeIds = new List<Guid>();
            yield return connection.SendReadTransaction(query, table =>
            {
                foreach (Dictionary<string, JToken> row in table)
                {
                    string guidString = (string)row["guid"];
                    nodeIds.Add(Guid.Parse(guidString));
                }
            });

            yield return "waiting for next frame :)";
            processNodeIds(nodeIds);
        }

        public IEnumerator ReadEmptyPathRoots(GraphProject project, Action<List<PathRoot>> processPathRoots)
        {
            string query = $"MATCH (project_root :PROJECT_ROOT {{guid: '{project.Id}'}})" +
                $"-[:HAS_PATH]-> (path_root :PATH_ROOT) " +
                $"RETURN path_root";

            List<PathRoot> pathRoots = new List<PathRoot>();
            yield return connection.SendReadTransaction(query, table =>
            {
                foreach (Dictionary<string, JToken> row in table)
                {
                    JObject pathRootJson = row["path_root"] as JObject;
                    pathRoots.Add(PathRoot.FromJObject(project, pathRootJson));
                }
            });

            yield return "waiting for next frame :)";
            processPathRoots(pathRoots);
        }

        public IEnumerator ReadEdgesBetweenNodesCo(GraphProject project, List<GraphNode> allNodes, Action<List<GraphEdge>> processGraphEdges) // works
        {
            string query = $"MATCH (:USER {{email: '{project.User.Email}'}}) " +
                $" -[:OWNS_PROJECT]-> (:PROJECT_ROOT {{title: '{project.Title}'}}) " +
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


                GraphNode parentNode = allNodes.Find(node => node.Id == parentId);
                if (parentNode == null)
                    throw new Exception($"Could not find parent node with id = {parentId}");

                GraphNode childNode = allNodes.Find(node => node.Id == childId);
                if (childNode == null)
                    throw new Exception($"Could not find child with id = {childId}");


                JObject edgeObj = row["edge"] as JObject;

                GraphEdge edge = GraphEdge.FromJObject(edgeObj, parentNode, childNode);

                edges.Add(edge);

            }
            processGraphEdges(edges);
        }

        // POTENTIAL BUG: this returns a list of NEWLY CREATED user objects. 
        // If there are two user objects corresponding to the same account
        // (i.e. two user objects with the same email)
        // then one will not necessarily have loaded in all the graphprojects that the other has
        // FIX: manage a global list of all users loaded in. pass in the list, and return a sublist
        public IEnumerator ReadUsersWithSharedAccess(GraphProject project, Action<List<GraphUser>> processUsersSharedWith)
        {
            string query = $"MATCH (project :PROJECT_ROOT {{guid: '{project.Id}'}}) " +
                $"-[:SHARED_WITH]-> (user :USER) " +
                $" RETURN user ";

            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, tableRead => table = tableRead);

            List<GraphUser> sharedUsers = table
            .Select(row => GraphUser.FromJObject(row["user"] as JObject))
            .ToList();

            yield return "waiting for next frame :)";

            processUsersSharedWith(sharedUsers);
        }

        public IEnumerator ReadProjectsSharedWith(GraphUser reader, Action<List<GraphProject>> processProjects)
        {
            string query = $"MATCH (owner :USER) " +
                "-[:OWNS_PROJECT]-> (project :PROJECT_ROOT) " +
                $"-[:SHARED_WITH]-> (reader :USER {{email: '{reader.Email}'}})" +
                $"RETURN owner, project";

            List<Dictionary<string, JToken>> table = null;
            yield return connection.SendReadTransaction(query, tableRead => table = tableRead);

            List<GraphProject> projects = new List<GraphProject>();
            foreach (var row in table)
            {
                GraphUser owner = GraphUser.FromJObject(row["owner"] as JObject);
                GraphProject project = GraphProject.FromJObject(owner, row["project"] as JObject);
                projects.Add(project);
            }

            processProjects(projects);
        }


        // ============================== UPDATE
        public IEnumerator UpdateNodeTitleCo(GraphNode node, string title) // Works!
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.title = '{title}'";

            LogNode logNode = new NodeUpdateLog(node, "title", title);
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }



        public IEnumerator UpdateNodeDescriptionCo(GraphNode node, string description) // Works!
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.description = '{description}'";

            LogNode logNode = new NodeUpdateLog(node, "description", description);
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }



        public IEnumerator UpdateNodeCoordinatesCo(GraphNode node, (double x, double y, double z) coordinates) // Works!
        {
            string query = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $" SET node.coordinates = [{coordinates.x}, {coordinates.y}, {coordinates.z}]";

            LogNode logNode = new NodeUpdateLog(node, "description", coordinates.ToString());
            yield return MakeAndLogChangeQueryCo(node.Project, query, logNode);
        }


        public IEnumerator UpdateEdgeTitleCo(GraphEdge edge, string title)
        {
            string updateTitleQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $"-[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}})" +
                $" SET edge.title = '{title}'";

            LogNode logNode = new EdgeUpdateLog(edge, "title", title);
            yield return MakeAndLogChangeQueryCo(edge.Project, updateTitleQuery, logNode);
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

            LogNode logNode = new EdgeUpdateLog(edge, "description", description);
            yield return MakeAndLogChangeQueryCo(edge.Project, updateDescQuery, logNode);
        }

        public IEnumerator ShareProject(GraphProject project, GraphUser toShareWith)
        {
            string query = $"MATCH (project :PROJECT_ROOT {{guid: '{project.Id}'}}), " +
                $"(user :USER {{email: '{toShareWith.Email}'}}) " +
                $"MERGE (project) -[:SHARED_WITH]-> (user)";
                Debug.Log(query);
            yield return connection.SendWriteTransactions(query);
        }

        public IEnumerator UnshareProject(GraphProject project, GraphUser toUnshareWith)
        {
            string query = $"MATCH (project :PROJECT_ROOT {{guid: '{project.Id}'}}) " +
                $" -[share :SHARED_WITH]-> (user :USER {{email: '{toUnshareWith.Email}'}}) " +
                $" DELETE share";

            yield return connection.SendWriteTransactions(query);
        }


        // ============================== DESTROY
        public IEnumerator DestroyNodeCo(GraphNode node) // works
        {
            string deleteQuery = $"MATCH (node :NODE {{guid: '{node.Id}'}}) " +
                $"DETACH DELETE (node)";

            LogNode logNode = new NodeDeletionLog(node);
            yield return MakeAndLogChangeQueryCo(node.Project, deleteQuery, logNode);
        }


        public IEnumerator DestroyEdgeCo(GraphEdge edge)
        {
            string deleteEdgeQuery = $"MATCH (:NODE {{guid: '{edge.Parent.Id}'}}) " +
                $" -[edge :LINK {{guid: '{edge.Id}'}}]-> " +
                $" (:NODE {{guid: '{edge.Child.Id}'}}) " +
                $" DELETE edge";

            LogNode logNode = new EdgeDeletionLog(edge);
            yield return MakeAndLogChangeQueryCo(edge.Project, deleteEdgeQuery, logNode);
        }

        private static string DestroyLogHistoryEdgeQuery(GraphProject project)
            => $"MATCH ({{title: '{project.Title}'}}) " +
                " -[r:LOG_HISTORY]->(n) " +
                " DELETE r";


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
            string query = $"MATCH (path_root :PATH_ROOT {{guid: '{path.Id}'}})" +
                $"DETACH DELETE path_root";

            yield return connection.SendWriteTransactions(query);
        }

        public IEnumerator DeleteGraphProject(GraphProject project)
        {
            string deleteNodesAndEdges = $"MATCH (project :PROJECT_ROOT {{guid: '{project.Id}'}}) " +
                $"-[:CONTAINS]-> (node :NODE)" +
                $"DETACH DELETE node";

            // string deleteLogNodes = $"MATCH (project :PROJECT_ROOT {{guid: '{project.Id}'}}) " +
            //     $"-[* :LOG_HISTORY]-> (logNode :LOG_NODE)" +
            //     $"DETACH DELETE logNode";

            string deletePaths = $"MATCH (project :PROJECT_ROOT {{guid: '{project.Id}'}}) " +
                $"-[:HAS_PATH]-> (path_root :PATH_ROOT) " +
                $"DETACH DELETE path_root";

            string deleteProject = $"MATCH (project :PROJECT_ROOT {{guid: '{project.Id}'}})" +
                $"DETACH DELETE project";

            yield return connection.SendWriteTransactions(deleteNodesAndEdges, deletePaths, deleteProject);
        }
    }
}