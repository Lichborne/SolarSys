using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Edge : MonoBehaviour
{
    GameObject edge;
    GameObject text;

public Edge(GameObject original, Vector3 position, Quaternion rotation) {
    edge = Instantiate(original, position, rotation);
  }
}

