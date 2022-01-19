using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    public class HelperFunctions
    {
        public static void PrintTreeFromNode(GraphNode CentralNode) // Starting at a central node in the tree, print the entire tree
        {
            foreach (var Edge in CentralNode.Edges)
            {
                Console.WriteLine("sup");
                if (Edge.Parent == CentralNode) // Get all edges for which node is parent
                {   
                    Console.WriteLine($"{Edge.Parent} {Edge} {Edge.Child}");
                    PrintTreeFromNode(Edge.Child);
                }
            }
        }

    }
}