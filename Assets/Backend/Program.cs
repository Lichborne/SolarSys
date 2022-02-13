﻿using System;
using static Backend.HelperFunctions;
using System.Collections.Generic;

namespace Backend
{
    class Program
    {
        public static List<(double x, double y, double z)> EquidistantPointsOnUnitSphere(int numPoints)
        {
            List<(double, double, double)> points = new List<(double, double, double)>();
            double phiInRadians = Math.PI * (3 - Math.Sqrt(5.0));

            for (int i = 0; i < numPoints; i++)
            {
                double y = 1 - ((double) i / (numPoints - 1)) * 2; // 1 >= y >= -1
                double radius = Math.Sqrt(1 - y * y);

                double theta = phiInRadians * i;
                double x = Math.Cos(theta);
                double z = Math.Sin(theta);
                points.Add((x, y, z));
            }

            return points;
        }

        // Prints `numPoints` evenly spaced points arranged on a sphere. We can position our GraphNodes in the database at these points to spread them out evenly for Front End
        public static void ShowPoints(int numPoints)
        {
            double scale = 7.0;
            foreach ((double x, double y, double z) in EquidistantPointsOnUnitSphere(numPoints))
                Console.WriteLine($"coordinates = [{scale * x :0.00}, {scale * y :0.00}, {scale * z :0.00}]");
        }

        public static void Main()
        {
            ShowPoints(7);

            GraphProject project = new GraphProject();
            // DatabaseView database = new DatabaseView("bolt://localhost:7687", "neo4j", "password");

            LogNode logNode = new LogNode(ChangeEnum.Update, "body");
            project.Database.AppendLogNode(project, logNode);


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
