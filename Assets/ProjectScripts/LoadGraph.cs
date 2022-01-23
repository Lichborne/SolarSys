using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Backend;
/*
public class LoadGraph : MonoBehaviour
{
    public GameObject planet;
    public GameObject sun;
    public GameObject edge;

    private int x = 0;

    private int y = 0;

    private int z = 0;

    // Start is called before the first frame update
    void Start()
    {
        using (var database = new DatabaseView("bolt://localhost:7687", "neo4j", "password")) 
        {
            List<GraphNode> startNodes = database.ReadStartingNodes();

            foreach (GraphNode startNode in startNodes) {

                    GameObject newsun = Instantiate(Node, new Vector3(x, y, z), Quaternion.identity);
                    x+=5; y+=5; z+=5;
                    ChangeInputFieldText(newsun, startNode.Text);

                    foreach (var edge in startNode.Edges) {
                        GameObject newedge = Instantiate(edge, new Vector3(x, y, z), Quaternion.identity)
                        edge.Child
                    }

            }
        }

        

    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
*/

