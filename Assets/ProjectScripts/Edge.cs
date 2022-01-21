using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Edge : MonoBehaviour
{

    public enum EdgeType 
    {
        RELATION,
        SIBLING,
        PARENTCHILD
    }

    GameObject edge;
    GameObject text;
    EdgeType type;

public Edge(GameObject original, Vector3 position, Quaternion rotation, EdgeType etype) {
    edge = Instantiate(original, position, rotation);
    type = etype;
  }
}

