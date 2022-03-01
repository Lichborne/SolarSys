using System;
using System.Linq;
using System.Collections.Generic;
using static Backend.StringExtensions;

using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;
namespace Backend
{
    public class NodeCreationSchema
    {
        [JsonProperty(Required = Required.Always)]
        public string Title;

        [JsonProperty(Required = Required.Always)]
        public string Description;

        [JsonProperty(Required = Required.Always)]
        public Guid Id;

        [JsonProperty(Required = Required.Always)]
        public (double X, double Y, double Z) Coordinates;
    }

    public class ValuePair<T>
    {
        [JsonProperty(Required = Required.Always)]
        T NewValue;

        [JsonProperty(Required = Required.Always)]
        T OldValue;
    }

    public class NodeUpdateSchema
    {
        ValuePair<String> Title;

        ValuePair<String> Body;

        [JsonProperty(Required = Required.Always)]
        Guid Id;

        ValuePair<(double X, double Y, double Z)> Coordinates;
    }

    public class EdgeCreationSchema
    {
        [JsonProperty(Required = Required.Always)]
        string Title;

        [JsonProperty(Required = Required.Always)]
        string Body;

        [JsonProperty(Required = Required.Always)]
        Guid Id;

        [JsonProperty(Required = Required.Always)]
        Guid ParentId;

        [JsonProperty(Required = Required.Always)]
        Guid ChildId;
    }

    public class EdgeUpdateSchema
    {
        [JsonProperty(Required = Required.Always)]
        Guid Id;

        ValuePair<string> Title;

        ValuePair<string> Body;

        ValuePair<Guid> ParentId;

        ValuePair<Guid> ChildId;
    }
}