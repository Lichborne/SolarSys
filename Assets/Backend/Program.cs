using System;

namespace Backend
{
    class Program
    {
        public static void Main()
        {
            using (var database = new DatabaseView("bolt://localhost:7687", "neo4j", "password"))
            {
                foreach (int id in database.AllNodeIds())
                    Console.WriteLine($"found id = {id}");
            }
        }
    }
}
