using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
namespace Backend
{
    // Represents an entire graph, consisting of GraphNodes and GraphEdges, that the user has made
    public class GraphProject : IGraphRegion
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public List<GraphUser> UsersSharedWith { get; private set; } = new List<GraphUser>();
        public List<GraphNode> Nodes { get; private set; } = new List<GraphNode>();
        public List<GraphEdge> Edges { get; private set; }  = new List<GraphEdge>();
        public List<PathRoot> Paths { get; private set; } = new List<PathRoot>();
        public bool IsEmpty { get => !Nodes.Any(); }
        public GraphUser User { get; private set; }

        public GraphProject(GraphUser user, Guid id, string title)
        {
            User = user;
            Id = id;
            Title = title;
        }

        public GraphProject(string userEmail, string title) :
            this(new GraphUser(userEmail), Guid.NewGuid(), title)
        { }

        public GraphProject(GraphUser user, string title) : 
            this(user, Guid.NewGuid(), title)
        { }

        public GraphProject Copy(GraphUser newUser, string newTitle)
        {
            GraphProject projectCopy = new GraphProject(newUser, newTitle);
            
            // creating copies of each old node, keeping track of which old node corresponds to which copy
            Dictionary<GraphNode, GraphNode> oldNodeToCopy = new Dictionary<GraphNode, GraphNode>();

            foreach (GraphNode oldNode in Nodes)
            {
                (float X, float Y, float Z) coordsCopy = oldNode.Coordinates;
                GraphNode copy = new GraphNode(Guid.NewGuid(), projectCopy, oldNode.Title, oldNode.Description, coordsCopy);
                oldNodeToCopy[oldNode] = copy;
            }

            // going over each edge in each old node
            // adding a corresponding path copy to the corresponding node copy
            List<GraphEdge> newEdges = new List<GraphEdge>();
            foreach (GraphNode oldNode in Nodes)
            {
                foreach (GraphEdge oldEdge in oldNode.Edges)
                {
                    GraphNode parentCopy = oldNodeToCopy[oldEdge.Parent];
                    GraphNode childCopy = oldNodeToCopy[oldEdge.Child];
                    GraphEdge edgeCopy = new GraphEdge(Guid.NewGuid(), oldEdge.Title, oldEdge.Description, parentCopy, childCopy);   

                    newEdges.Add(edgeCopy);
                }
            }

            projectCopy.Nodes.AddRange(oldNodeToCopy.Values);
            projectCopy.Edges.AddRange(newEdges);
            return projectCopy;
        }

        // Reads nodes, edges and paths from database
        private IEnumerator ReadSelfFromDatabase()
        {
            yield return User.Database.ReadNodesFromProjectCo(this, nodesRead => Nodes = nodesRead);
            yield return User.Database.ReadEdgesBetweenNodesCo(this, Nodes, edgesRead => Edges = edgesRead);

            foreach (var edge in Edges)
                edge.Parent.Edges.Add(edge);

            yield return User.Database.ReadUsersWithSharedAccess(this, users => UsersSharedWith = users);
            yield return User.Database.ReadEmptyPathRoots(this, pathsRead => Paths = pathsRead);
        }

        public IEnumerator ReadFromDatabase(Action<GraphProject> processReadProject = null)
        {
            // Read in nodes and edges and process a graph project
            yield return ReadSelfFromDatabase();

            if (processReadProject != null)
            {
                yield return "proceed to next frame";
                processReadProject(this);
            }
        }

        public IEnumerator ReadFromDatabase(Action<IGraphRegion> processReadRegion = null)
        {
            yield return ReadSelfFromDatabase();
            if (processReadRegion != null)
            {
                yield return "waiting for next frame :)";
                processReadRegion(this);
            }
        }

        public IEnumerator ShareWith(GraphUser user)
        {
            UsersSharedWith.Add(user);

            yield return User.Database.ShareProject(this, user);
        }

        public IEnumerator UnshareWith(GraphUser user)
        {
            if (!IsSharedWith(user))
                throw new Exception($"Could not unshare project {Title} with user {user.Email} as the user is not shared with the project.");
            
            UsersSharedWith.RemoveAll(u => u.Email == user.Email);
            yield return User.Database.UnshareProject(this, user);
        }

        public bool IsOwnedBy(GraphUser user)
            => User.Email == user.Email;
        
        public bool IsSharedWith(GraphUser user)
            => UsersSharedWith.Any(u => u.Email == user.Email);

        public static GraphProject FromJObject(GraphUser user, JObject json)
        {
            string guidString = (string) json["guid"];
            string title = (string) json["title"];
            return new GraphProject(user, Guid.Parse(guidString), title);
        }

        public IEnumerator CreateCopyInDatabase(GraphUser newUser, string newTitle, Action cleanupFunc = null)
        {
            yield return ReadFromDatabase(null);
            
            GraphProject copy = Copy(newUser, newTitle);
            if (!newUser.Projects.Any(proj => proj.Id == Id))
                newUser.Projects.Add(this); // bad
            
            yield return copy.CreateInDatabase();

            if (cleanupFunc != null)
                cleanupFunc();
        }

        public IEnumerator CreateInDatabase(Action cleanupFunc)
        {
            yield return User.Database.CreateBlankGraphProject(this);
            
            if (!User.Projects.Any(proj => proj.Id == Id))
                User.Projects.Add(this);

            foreach (GraphNode node in Nodes)
                yield return node.CreateInDatabase();
            
            foreach (GraphEdge edge in Edges)
                yield return edge.CreateInDatabase();

            if (cleanupFunc != null)
                cleanupFunc();
        }

        public IEnumerator CreateInDatabase()
            => CreateInDatabase(null);

        public IEnumerator DeleteFromDatabase(Action cleanupFunc)
        {
            yield return User.Database.DeleteGraphProject(this);
            User.Projects.RemoveAll(proj => proj.Id == Id);
            if (cleanupFunc != null)
                cleanupFunc();
        }

        public IEnumerator DeleteFromDatabase()
            => DeleteFromDatabase(null);
    }
}