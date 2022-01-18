﻿using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    public class DatabaseView : IDisposable
    {
        private bool _disposed = false;
        private IDriver _driver;

        ~DatabaseView()
            => Dispose(false);

        public DatabaseView(string uri, string username, string password)
            => _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(username, password));

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

        /// <summary> IDs of the nodes that <paramref name="nodeId"/> links to. </summary>
        public List<int> NodeIdsLinkedFrom(int nodeId)
            => NodeIdsMatchingQuery($"match (linkingNode) --> (baseNode) where id(linkingNode) = {nodeId} return id(baseNode)");

        public GraphNode FindNodeByGuid(Guid id)
        {
            var query = $"match (node: {{guid: '{id}'}}) return node";
            using var session = _driver.Session();

            return session.ReadTransaction(tx => 
            {
                var result = tx.Run(query);
                var record = result.Single();

                string text = record["text"].As<string>();
                Guid id = Guid.Parse(record["guid"].As<string>()); 
                List<double> coords = record["coordinates"].As<List<double>>();

                return new GraphNode(text, id, (coords[0], coords[1], coords[2]));               
            });
        }

    /*
        public NodeTree CreateNodeTreeFromGuid(string guid)
        {
            var query = $"match (n {{'guid': {guid}}}) return n";
            using var session = _driver.Session();

            NodeTree nodeTree = session.ReadTransaction(tx =>
            {
                var result = tx.Run(query);
                var record = result.Single();
                var content = record["content"].As<string>();
                var coordinates = record["coordinates"].As<List<double>>();
                return new NodeTree(coordinates, content, guid);
            }).ToList();

            return nodeTree;
        } */

        /// <summary> IDs of the nodes that link to <paramref name="nodeId"/>. </summary>
        public List<int> NodeIdsLinkingTo(int nodeId)
            => NodeIdsMatchingQuery($"match (linkingNode) --> (baseNode) where id(baseNode) = {nodeId} return id(linkingNode)");

        /// <summary> The IDs of every node in the database. </summary>
        public List<int> AllNodeIds()
            => NodeIdsMatchingQuery("match (x) return id(x)");

        /// <summary> The IDs returned by <paramref name="query"/>. <paramref name="query"/> must return node IDs. </summary>
        private List<int> NodeIdsMatchingQuery(string query)
        {
            using var session = _driver.Session();

            List<int> nodeIds = session.ReadTransaction(tx =>
            {
                var result = tx.Run(query);
                List<int> ids = new List<int>();

                foreach (var record in result)
                    ids.Add(record[0].As<int>());

                return ids;
            }).ToList();

            return nodeIds;
        }  
    }
}
