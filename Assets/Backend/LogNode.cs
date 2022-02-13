using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using static Backend.StringExtensions;

namespace Backend
{
    public enum ChangeEnum
    {
        Create,
        Update,
        Delete
    }
    public class LogNode
    {
        public ChangeEnum Change { get; private set; }
        public string Body { get; private set; }

        public Guid Id { get; private set; }

        public DateTime TimeStamp { get; private set; }

        // public LogNode NextNode { get; private set; }

        public LogNode(ChangeEnum change, string body)
        {
            Id = Guid.NewGuid();
            Change = change;
            Body = body;
            TimeStamp = DateTime.UtcNow;
        }

        public LogNode(ChangeEnum change, string body, Guid id, DateTime timeStamp)
        {
            Id = id;
            Change = change;
            Body = body;
            TimeStamp = timeStamp;
        }

        public static LogNode FromINode(INode dbNode)
        {
            Enum.TryParse(dbNode.Properties["change"].As<string>(), out ChangeEnum change);
            string body = dbNode.Properties["body"].As<string>();
            Guid guid = Guid.Parse(dbNode.Properties["guid"].As<string>());
            DateTime timeStamp = DateTime.Parse(dbNode.Properties["timestamp"].As<string>());

            return new LogNode(change, body, guid, timeStamp);
        }

    }


}