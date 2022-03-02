using System;
using System.Linq;
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

        // public static GraphNode FromINode(GraphProject project, INode dbNode)
        // {
        //     string title = dbNode.Properties["title"].As<string>();
        //     string description = dbNode.Properties["description"].As<string>();
        //     string guidText = dbNode.Properties["guid"].As<string>();
        //     Guid guid = Guid.Parse(guidText);
        //     List<float> coords = dbNode.Properties["coordinates"].As<List<float>>();

        //     return new GraphNode(guid, project, title, description, (coords[0], coords[1], coords[2]));
        // }

        public static GraphNode FromJObject(GraphProject project, JObject json)
        {
            string title = (string)json["title"];
            string description = (string)json["description"];
            Guid guid = Guid.Parse((string)json["guid"]);
            List<float> coords = (json["coordinates"] as JArray)
                .Select(c => (float)c)
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
        public IEnumerator AddEdgeCo(GraphEdge edge) // works
        {
            yield return Project.User.Database.CreateParentChildRelationshipCo(this, edge, edge.Child);
            Edges.Add(edge);
        }

        // Removes the edge from the node. DOES NOT WRITE TO DATABASE YET
        public void RemoveEdge(GraphEdge edge)
            => Edges.Remove(edge);


        public IEnumerator UpdateTitle(string title)
        {
            Title = title;
            yield return Project.User.Database.UpdateNodeTitleCo(this, title);
        }

        public IEnumerator UpdateTitleCo(string title) // Works!
        {
            yield return Project.User.Database.UpdateNodeTitleCo(this, title);
            Title = title;
        }

        public IEnumerator UpdateDescription(string description)
        {
            Description = description;
            yield return Project.User.Database.UpdateNodeDescriptionCo(this, description);
        }

        public IEnumerator UpdateDescriptionCo(string description) // Works!
        {
            Description = description;
            yield return Project.User.Database.UpdateNodeDescriptionCo(this, description);
        }

        public IEnumerator UpdateCoordinatesCo((float x, float y, float z) coordinates) // Works!
        {
            Coordinates = coordinates;
            yield return Project.User.Database.UpdateNodeCoordinatesCo(this, coordinates);
        }

        // deletes the node from the database along with its edges. does not affect the node's children
        public IEnumerator DeleteFromDatabaseCo() // works
            => Project.User.Database.DestroyNodeCo(this);


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

        public String Serialize()
        {
            return $"{Title};{Description};{Id.ToString()};{Coordinates.ToString()}";
        }
    }
}