using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using static Backend.StringExtensions;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System.Collections;

namespace Backend
{
    public class GraphNode : IEquatable<GraphNode>
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid Id { get; private set; }

        public (float X, float Y, float Z) Coordinates { get; private set; }

        public GraphProject Project { get; private set; }
        public List<GraphEdge> Edges = new List<GraphEdge>();

        public GraphNode(Guid id, GraphProject project, string title, string description, (float x, float y, float z) coordinates)
        {
            Title = title;
            Description = description;
            Id = id;
            Coordinates = coordinates;
            Project = project;
        }

        public GraphNode(GraphProject project, string title, string description, (float x, float y, float z) coordinates) :
            this(Guid.NewGuid(), project, title, description, coordinates)
        { }

        public static GraphNode FromINode(GraphProject project, INode dbNode)
        {
            string title = dbNode.Properties["title"].As<string>();
            string description = dbNode.Properties["description"].As<string>();
            string guidText = dbNode.Properties["guid"].As<string>();
            Guid guid = Guid.Parse(guidText);
            List<float> coords = dbNode.Properties["coordinates"].As<List<float>>();

            return new GraphNode(guid, project, title, description, (coords[0], coords[1], coords[2]));
        }

        public static GraphNode FromJObject(GraphProject project, JObject json)
        {
            string title = (string) json["title"];
            string description = (string) json["description"];
            Guid guid = Guid.Parse((string) json["guid"]);
            List<float> coords = (json["coordinates"] as JArray)
                .Select(c => (float) c)
                .ToList();
            
            return new GraphNode(guid, project, title, description, (coords[0], coords[1], coords[2]));
        }

        //  Writes the node, with no edges, to the database
        public IEnumerator CreateInDatabase()
            => Project.User.Database.CreateUnlinkedNode(this);


        public IEnumerator CreateInDatabaseCo()
        {
            yield return Project.User.Database.CreateUnlinkedNodeCo(this);
        }

        // Adds an extra edge to the node, writing it to the database
        public void AddEdge(GraphEdge edge)
        {
            Project.User.Database.CreateParentChildRelationship(this, edge, edge.Child);
            Edges.Add(edge);
        }

        public IEnumerator AddEdgeCo(GraphEdge edge) // works
        {
            yield return Project.User.Database.CreateParentChildRelationshipCo(this, edge, edge.Child);
            Edges.Add(edge);
        }

        // Removes the edge from the node. DOES NOT WRITE TO DATABASE YET
        public void RemoveEdge(GraphEdge edge)
            => Edges.Remove(edge);


        public void UpdateTitle(string title)
        {
            Project.User.Database.UpdateNodeTitle(this, title);
            Title = title;
        }

        public IEnumerator UpdateTitleCo(string title) // Works!
        {
            yield return Project.User.Database.UpdateNodeTitleCo(this, title);
            Title = title;
        }

        public void UpdateDescription(string description)
        {
            Project.User.Database.UpdateNodeDescription(this, description);
            Description = description;
        }

        public IEnumerator UpdateDescriptionCo(string description) // Works!
        {
            yield return Project.User.Database.UpdateNodeDescriptionCo(this, description);
            Description = description;

        }

        public void UpdateCoordinates((float x, float y, float z) coordinates)
        {
            Project.User.Database.UpdateNodeCoordinates(this, coordinates);
            Coordinates = coordinates;
        }

        public IEnumerator UpdateCoordinatesCo((float x, float y, float z) coordinates) // Works!
        {
            yield return Project.User.Database.UpdateNodeCoordinatesCo(this, coordinates);
            Coordinates = coordinates;
        }

        // deletes the node from the database. does not affect the node's edges or children
        public void DeleteFromDatabase()
            => Project.User.Database.DestroyNode(this);

        public IEnumerator DeleteFromDatabaseCo() // works
        {
            yield return Project.User.Database.DestroyNodeCo(this);
        }


        public override string ToString()
            => $"({Id.ToString().Truncate(5)}: {Title.Truncate(20)})";

        public bool Equals(GraphNode other)
            => Id == other.Id;

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals(obj as GraphNode);
        }

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}