using System;
using System.Linq;
using Neo4j.Driver;
using System.Collections;
using System.Collections.Generic;
using static Backend.StringExtensions;

namespace Backend
{
    public class GraphUser 
    {
        public string Email { get; private set; }
        public List<GraphProject> Projects = new List<GraphProject>();
        public DatabaseView Database = new DatabaseView();

        public bool IsEmpty { get => !Projects.Any(); }

        public GraphUser(string email = "foo.bar@doc.ic.ac.uk")
        {
            this.Email = email;
        }

        // Reads in title and ID of each graph project into Projects, but leaves them empty (does not load the nodes or edges)
        // Each empty project can then be read in by starting the coroutine Projects[i].ReadFromDatabase()
        public IEnumerator ReadAllEmptyProjects(Action<GraphUser> processUser)
        {
            yield return Database.ReadAllEmptyProjects(this, projectsRead => Projects = projectsRead);
            if (processUser != null)
            {
                yield return "waiting for next frame :)";
                processUser(this);
            }
        }
        /*
        public List<GraphProject> returnProjectsWithTitle(List<String> projectTitles)
        {
            List<GraphProject> projects = new List<GraphProject>();
            foreach (var projectTitle in projectTitles)
            {
                GraphProject newGraphProject = new GraphProject(Email, projectTitle);
                projects.Add(newGraphProject);
                Projects.Add(newGraphProject); // Store in class as well for future access
            }
            return projects;
        } */
    }
}
