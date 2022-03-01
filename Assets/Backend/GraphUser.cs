using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static Backend.StringExtensions;
using Newtonsoft.Json.Linq;

namespace Backend
{
    public class GraphUser : IEquatable<GraphUser>
    {
        public string Email { get; private set; }
        public List<GraphProject> Projects = new List<GraphProject>();
        public List<GraphProject> ProjectsSharedWith = new List<GraphProject>();
        public DatabaseView Database { get; private set; } = new DatabaseView();

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
            yield return Database.ReadEmptyProjectsSharedWith(this, sharedProjects => ProjectsSharedWith = sharedProjects);

            if (processUser != null)
            {
                yield return "waiting for next frame :)";
                processUser(this);
            }
        }

        public static GraphUser FromJObject(JObject json)
            => new GraphUser((string) json["email"]);
        

        public bool Equals(GraphUser other)
            => Email == other.Email;
        

        public override bool Equals(object other)
        {
            if (!(other is GraphUser))
                return false;
            
            return Equals(other as GraphUser);
        }


        public override int GetHashCode()
            => Email.GetHashCode();

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
