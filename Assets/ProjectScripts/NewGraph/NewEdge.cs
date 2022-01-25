using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEdge : MonoBehaviour
{

    private GameObject _edgePrefab = null;

    private GameObject _selfReferencePrefab = null;

    private GameObject edge = null;

    public GameObject _parent { get; set; } = null;

    public GameObject _child {get; set; } = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Instead of update; moving called by funct in node.
    void Update()
    {   
        if(edge == null  || _edgePrefab == null || _selfReferencePrefab == null)
            return;

        if (_parent == _child) {
            edge.transform.position = new Vector3(_parent.transform.position.x, _parent.transform.position.y, _parent.transform.position.z);
            edge.transform.LookAt(_parent.transform);
            return;
        }

        edge.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // rotate towards child
        edge.transform.LookAt(_child.transform);

        //for scaling
        Vector3 ls = edge.transform.localScale;

        // get scaling measure
        ls.z = Vector3.Distance(edge.transform.position, _child.transform.position);

        //scale
        edge.transform.localScale = ls;

        //move into position
        edge.transform.position = new Vector3(
              (_child.transform.position.x+_parent.transform.position.x)/2,
					    (_child.transform.position.y+_parent.transform.position.y)/2,
					    (_child.transform.position.z+_parent.transform.position.z)/2);
  }

    public void InstantiateEdge(GameObject epf, GameObject srpf, GameObject p, GameObject c) { 
        _edgePrefab = epf;
        _selfReferencePrefab = srpf;
        _parent = p;
        _child = c;

        if (_parent == _child) {
                edge = Instantiate(_selfReferencePrefab, _parent.transform.position, _parent.transform.rotation);
                return;
            }

        edge = Instantiate(_edgePrefab, _parent.transform.position, _parent.transform.rotation);
    }
}
