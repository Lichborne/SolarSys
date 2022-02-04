using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using static Backend.StringExtensions;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        public ChangeEnum Change { get; private set; }

        public Guid Id { get; private set; }

        public DateTime TimeStamp { get; private set; }

        // public LogNode NextNode { get; private set; } // TODO 

        public string? Log;

        public LogNode(ChangeEnum change)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTime.UtcNow;
        }

        public LogNode(ChangeEnum change, string log, Guid id, DateTime timeStamp)
        {
            Change = change;
            Log = log;
            Id = id;
            TimeStamp = timeStamp;
        }

        public static LogNode FromINode(INode dbNode)
        {
            Enum.TryParse(dbNode.Properties["change"].As<string>(), out ChangeEnum change);
            string log = dbNode.Properties["body"].As<string>();
            Guid id = Guid.Parse(dbNode.Properties["guid"].As<string>());
            DateTime timeStamp = DateTime.Parse(dbNode.Properties["timestamp"].As<string>());

            return new LogNode(change, log, id, timeStamp);
        }

    }

    public class CreationLog
    {
        string Title;
        string Body;
        Guid Id;
        (double X, double Y, double Z) Coordinates;

        public CreationLog(string title, string body, Guid id, (double X, double Y, double Z) coordinates)
        {
            Title = title;
            Body = body;
            Id = id;
            Coordinates = coordinates;
        }
    }

    public class NodeCreationLog : LogNode
    {
        public NodeCreationLog(CreationLog log) : base(ChangeEnum.AddNode)
        {
            Log = JsonSerializer.Serialize(log);
        }
    }

    public class NullableNodeSchema
    {
        // not feasible, can still create instance w/o guid
        string? Title;
        string? Body;
        Guid Id;
        (double X, double Y, double Z)? Coordinates;
    }
    public class UpdateLog
    {
        NullableNodeSchema NewValues;
        NullableNodeSchema OldValues;

        public UpdateLog(NullableNodeSchema newValues, NullableNodeSchema oldValues)
        {
            NewValues = newValues;
            OldValues = oldValues;
        }
    }

    public class NodeUpdateLog : LogNode
    {

        public NodeUpdateLog(UpdateLog log) : base(ChangeEnum.UpdateNode)
        {
            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            Log = JsonSerializer.Serialize(log, options);
        }
    }




}