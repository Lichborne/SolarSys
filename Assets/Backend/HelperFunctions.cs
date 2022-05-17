// using System;
// using System.Collections.Generic;
// using System.Linq;

// namespace Backend
// {
//     public static class HelperFunctions
//     {
//         public static void PrintTreeFromNode(GraphNode centralNode)
//         {
//             HashSet<GraphEdge> edgesVisited = new HashSet<GraphEdge>();
//             PrintTreeFromNode(centralNode, edgesVisited);
//         }

//         public static void PrintTreeFromNode(GraphNode CentralNode, HashSet<GraphEdge> edgesVisited) // Starting at a central node in the tree, print the entire tree
//         {
//             foreach (var Edge in CentralNode.Edges)
//             {
//                 if (!edgesVisited.Contains(Edge)) // Get all edges for which node is parent
//                 {
//                     Console.WriteLine($"{Edge.Parent} {Edge} {Edge.Child}");
//                     edgesVisited.Add(Edge);
//                     if (Edge.Parent == CentralNode){PrintTreeFromNode(Edge.Child, edgesVisited);}
//                     else PrintTreeFromNode(Edge.Parent, edgesVisited);
//                 }
//             }
//         }

//         private static void VisitSection(this GraphNode centralNode, Action<GraphEdge> visitor, HashSet<GraphEdge> edgesVisited)
//         {
//             foreach (var edge in centralNode.Edges)
//             {
//                 if (!edgesVisited.Contains(edge))
//                 {
//                     visitor(edge);
//                     edgesVisited.Add(edge);
//                     edge.GetAttachedNode(centralNode).VisitSection(visitor, edgesVisited);
//                 }
//             }
//         }

//         public static void VisitSection(this GraphNode centralNode, Action<GraphEdge> visitor)
//             => centralNode.VisitSection(visitor, new HashSet<GraphEdge>()); 

//     }
// }