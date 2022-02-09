// using System;
// using System.Linq;
// using Neo4j.Driver;
// using System.Collections.Generic;
// using static Backend.StringExtensions;

// namespace backend
// {
//     public class GraphUser 
//     {
//         private static String userEmail;
        
//         public List<GraphProject> userGraphProjects = new List<GraphProject>();

//         private List<String> projectTitles = new List<String>();
//         public GraphUser(String userEmail, string dbUri = /* "bolt://localhost:7687" */ "neo4j://cloud-vm-42-36.doc.ic.ac.uk:7687", string dbUsername = "neo4j", string dbPassword = "s3cr3t")
//         {
//             Database = new DatabaseView(dbUri, dbUsername, dbPassword);
//             projectTitles = Database.ReadAllProjectTitlesAttachedToUser(userEmail);

//             foreach (var projectTitle in projectTitles)
//             {
//                 GraphProject newGraphProject = new GraphProject(userEmail = userEmail, projectTitle = projectTitle);
//                 userGraphProjects.Add(newGraphProject);
//             }
//         }
//     }
// }
