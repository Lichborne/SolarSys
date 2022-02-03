using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEndEdge : MonoBehaviour
{
    private Backend.GraphEdge _databaseEdge = null;

    public GameObject _parent { get; set; } = null;

    public GameObject _child {get; set; } = null;

    // Instead of update; moving called by funct in node.
    void Update()
    {   
        if (_databaseEdge == null) {
            return;
        }
        //if it's a self reference edge, we just update its position in a more simple manner and return
        if (_parent == _child) {
            gameObject.transform.position = new Vector3(_parent.transform.position.x, _parent.transform.position.y, _parent.transform.position.z);
            gameObject.transform.LookAt(_parent.transform);
            return;
        }

        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // rotate towards child
        gameObject.transform.LookAt(_child.transform);

        //for scaling
        Vector3 ls = gameObject.transform.localScale;

        // get scaling measure
        ls.z = Vector3.Distance(gameObject.transform.position, _child.transform.position);

        //scale
        gameObject.transform.localScale = ls;

        //move into position
        gameObject.transform.position = new Vector3(
            (_child.transform.position.x+_parent.transform.position.x)/2,
            (_child.transform.position.y+_parent.transform.position.y)/2,
            (_child.transform.position.z+_parent.transform.position.z)/2);
  }
    // constructors are ill advised, so we use this instead
    public void InstantiateEdge(Backend.GraphEdge graphEdge, GameObject p, GameObject c) { 

        _databaseEdge = graphEdge;
        _parent = p;
        _child = c;

        gameObject.GetComponent<StoreParentChild>().parent = _parent;
        gameObject.GetComponent<StoreParentChild>().child = _child;
    }
}