using System;
using static Backend.HelperFunctions;

namespace Backend
{
    class Program
    {
        public static void Main()
        {
            Graph graph = new Graph(); 

            Console.WriteLine("Displaying all nodes");
            foreach (GraphNode node in graph.Nodes)
                Console.WriteLine($"{node}"); 
            
            Console.WriteLine("\nDisplaying all edges");
            foreach (GraphEdge edge in graph.Edges)
                Console.WriteLine($"{edge.Parent} {edge} {edge.Child}"); 

            
            Console.WriteLine("\nDouble checking that each node has its edges");
            foreach (GraphNode node in graph.Nodes)
                foreach (GraphEdge edge in node.Edges)
                    Console.WriteLine($"{edge.Parent} {edge} {edge.Child}"); 
        }
    }
}
