using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using static Backend.StringExtensions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;


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

        public string Log;

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
        public static void Validate<T>(T obj)
        {
            var results = new List<ValidationResult>();

            var validate = Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);

            if (!validate)
            {
                Console.WriteLine(String.Join("\n", results.Select(o => o.ErrorMessage)));
                throw new InvalidDataException();
            }
        }
    }

    public class NodeCreationLog : LogNode
    {
        public NodeCreationLog(NodeCreationSchema log) : base(ChangeEnum.AddNode)
        {
            Validate(log);
            Log = JsonSerializer.Serialize(log);
        }
    }

    public class EdgeCreationLog : LogNode
    {
        public EdgeCreationLog(EdgeCreationSchema log) : base(ChangeEnum.AddEdge)
        {
            Validate(log);
            Log = JsonSerializer.Serialize(log);
        }
    }

    public class NodeUpdateLog : LogNode
    {
        public NodeUpdateLog(NodeUpdateSchema log) : base(ChangeEnum.UpdateNode)
        {
            Validate(log);
            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            Log = JsonSerializer.Serialize(log, options);
        }
    }

    public class EdgeUpdateLog : LogNode
    {
        public EdgeUpdateLog(EdgeUpdateSchema log) : base(ChangeEnum.UpdateEdge)
        {
            Validate(log);
            var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
            Log = JsonSerializer.Serialize(log, options);
        }
    }

    public class NodeDeletionLog : LogNode
    {
        public NodeDeletionLog(NodeCreationSchema log) : base(ChangeEnum.DeleteNode)
        {
            Validate(log);
            Log = JsonSerializer.Serialize(log);
        }
    }

    public class EdgeDeletionLog : LogNode
    {
        public EdgeDeletionLog(EdgeCreationSchema log) : base(ChangeEnum.DeleteEdge)
        {
            Validate(log);
            Log = JsonSerializer.Serialize(log);
        }
    }
}