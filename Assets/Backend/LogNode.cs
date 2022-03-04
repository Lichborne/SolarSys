using System;
using System.Linq;
using System.Collections.Generic;
using static Backend.StringExtensions;

using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace Backend
{
    public enum ChangeEnum
    {
        AddNode,
        AddEdge,
        UpdateNode,
        UpdateEdge,
        DeleteNode,
        DeleteEdge
    }
    public class LogNode
    {
        public string Body { get; set; }
        public ChangeEnum Change { get; private set; }
        public Guid Id { get; private set; }
        public DateTime TimeStamp { get; private set; }


        public LogNode(ChangeEnum change)
        {
            Change = change;
            Id = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }

        // Old code , to delete
        public LogNode(ChangeEnum change, string body)
        {
            Change = change;
            Body = body;
        }

        public LogNode(string body, ChangeEnum change, Guid id, DateTime timeStamp)
        {
            Body = body;
            Change = change;
            Id = id;
            TimeStamp = timeStamp;
        }

        public static LogNode FromJObject(GraphProject project, JObject obj)
        {
            Enum.TryParse((string)obj["change"], out ChangeEnum change);
            string body = (string)obj["body"];
            Guid guid = Guid.Parse((string)obj["guid"]);
            DateTime timeStamp = DateTime.Parse((string)obj["timestamp"]);

            return new LogNode(body, change, guid, timeStamp);
        }
    }
    public class NodeCreationLog : LogNode
    {
        public NodeCreationLog(GraphNode node) : base(ChangeEnum.AddNode)
        {
            Body = node.Serialize();
        }
    }

    public class EdgeCreationLog : LogNode
    {
        public EdgeCreationLog(GraphNode parent, GraphEdge edge, GraphNode child) : base(ChangeEnum.AddEdge)
        {
            Body = $"({parent.Id.ToString()},{edge.Serialize()},{child.Id.ToString()})";
        }
    }

    public class NodeUpdateLog : LogNode
    {
        public NodeUpdateLog(GraphNode node, String field, String newValue) : base(ChangeEnum.UpdateNode)
        {
            Body = $"({node.Serialize()},{field},{newValue})";
        }
    }

    public class EdgeUpdateLog : LogNode
    {
        public EdgeUpdateLog(GraphEdge edge, String field, String newValue) : base(ChangeEnum.UpdateEdge)
        {

            Body = $"({edge.Serialize()},{field},{newValue})";
        }
    }

    public class NodeDeletionLog : LogNode
    {
        public NodeDeletionLog(GraphNode node) : base(ChangeEnum.DeleteNode)
        {
            Body = node.Serialize();
        }
    }

    public class EdgeDeletionLog : LogNode
    {
        public EdgeDeletionLog(GraphEdge edge) : base(ChangeEnum.DeleteEdge)
        {
            Body = edge.Serialize();
        }
    }
}