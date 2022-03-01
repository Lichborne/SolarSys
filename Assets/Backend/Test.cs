// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;
// using Backend;
// using Newtonsoft.Json.Linq;

// public class Test : MonoBehaviour
// {
//     IEnumerator Start()
//     {
//         GraphUser user = new GraphUser();
//         yield return user.ReadAllEmptyProjects(user => 
//         {
//             Debug.Log($"Test() found {user.Projects.Count} projects from user (which do not have their nodes / edges loaded in yet)");
//         });

//         foreach (GraphProject project in user.Projects)
//         {
//             yield return project.ReadFromDatabase();
//             Debug.Log($"Project '{project.Title}' has now loaded in {project.Nodes.Count} nodes");
//         }

//         GraphProject testProject = user.Projects.Find(proj => proj.Nodes.Count > 0);
//         // The GraphProject's path is initially empty. It has its Title and Id, but no nodes or edges
//         Debug.Log($"'{testProject.Title}' has {testProject.Paths.Count} paths (which do not have their nodes / edges loaded in yet)");
//         foreach (PathRoot path in testProject.Paths)
//         {
//             path.ReadFromDatabase();
//             Debug.Log($"Path '{path.Title}' has now loaded in {path.Nodes.Count} nodes");
//         }
//     }
// }