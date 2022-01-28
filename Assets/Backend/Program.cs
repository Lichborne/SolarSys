using System;
using static Backend.HelperFunctions;

namespace Backend
{
    class Program
    {
        public static void Main()
        {
            GraphProject graph = new GraphProject(); 
            /*
            Console.WriteLine("Displaying all nodes");
            foreach (GraphNode node in graph.Nodes)
                Console.WriteLine($"{node}"); 
            
            Console.ReadKey();
            Console.WriteLine("\nDisplaying all edges");
            foreach (GraphEdge edge in graph.Edges)
                Console.WriteLine($"{edge.Parent} {edge} {edge.Child}"); 

            Console.ReadKey();
            Console.WriteLine("\nDouble checking that each node has its edges");
            foreach (GraphNode node in graph.Nodes)
                foreach (GraphEdge nodeEdge in node.Edges)
                    Console.WriteLine($"{node} {nodeEdge} {nodeEdge.Child}");  */
        }
    }
}
