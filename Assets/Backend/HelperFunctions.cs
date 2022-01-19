using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend
{
    public class HelperFunctions
    {
        public static void PrintTreeFromNode(GraphNode centralNode)
        {
            HashSet<GraphEdge> edgesVisited = new HashSet<GraphEdge>();
            PrintTreeFromNode(centralNode, edgesVisited);
        }

        public static void PrintTreeFromNode(GraphNode CentralNode, HashSet<GraphEdge> edgesVisited) // Starting at a central node in the tree, print the entire tree
        {
            foreach (var Edge in CentralNode.Edges)
            {
                if (Edge.Parent == CentralNode && !edgesVisited.Contains(Edge)) // Get all edges for which node is parent
                {   
                    Console.WriteLine($"{Edge.Parent} {Edge} {Edge.Child}");
                    edgesVisited.Add(Edge);
                    PrintTreeFromNode(Edge.Child);
                }
            }
        }

    }
}