using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections.Generic;
using static Backend.StringExtensions;

namespace Backend
{
    public class GraphUser 
    {
        public String userEmail {get; private set;}
        
        public List<GraphProject> userGraphProjects = new List<GraphProject>();

        private List<String> projectTitles = new List<String>();
        public GraphUser(String userEmail, string dbUri = /* "bolt://localhost:7687" */ "neo4j://cloud-vm-42-36.doc.ic.ac.uk:7687", string dbUsername = "neo4j", string dbPassword = "s3cr3t")
        {
            this.userEmail = userEmail;
            var Database = new DatabaseView(dbUri, dbUsername, dbPassword);
            var projectTitles = Database.ReadAllProjectTitlesAttachedToUser(userEmail);
        }

        public List<GraphProject> returnProjectsWithTitle(List<String> projectTitles)
        {
            List<GraphProject> projects = new List<GraphProject>();
            foreach (var projectTitle in projectTitles)
            {
                GraphProject newGraphProject = new GraphProject(userEmail, projectTitle);
                projects.Add(newGraphProject);
                userGraphProjects.Add(newGraphProject); // Store in class as well for future access
            }
            return projects;
        }
    }
}
