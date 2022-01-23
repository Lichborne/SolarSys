using System;

namespace Backend
{
    class Program
    {
        public static void Main()
        {
            using (var database = new DatabaseView("bolt://localhost:7687", "neo4j", "password"))
            {
                GraphNode root = database.ReadNodeWithGuid(Guid.Parse("20d39f6b-8662-4328-8dc5-df57eb3c4a3a"));
                database.AddAllNodesLinkedToRoot(root);
                HelperFunctions.PrintTreeFromNode(root);
            }
        }
    }
}
