using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrontEndEdge : MonoBehaviour
{
    public bool _isCurved = false;

    public Backend.GraphEdge _databaseEdge { get; set; } = null;

    public GameObject _textObject { get; set; } = null;

    public GameObject _parent { get; set; } = null;

    public GameObject _child {get; set; } = null;

    private float _rotation = 0;

    // Instead of update; moving called by funct in node.
    void Start() 
    {
        
    }
    void Update()
    {   
        if (_databaseEdge == null && !(_textObject == null)) {
            return;
        }
        //if it's a self reference edge, we just update its position in a more simple manner and return
        if (_parent == _child) {
            gameObject.transform.position = new Vector3(_parent.transform.position.x, _parent.transform.position.y, _parent.transform.position.z);
            //gameObject.transform.LookAt(_parent.transform);
            return;
        }

        gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        // rotate towards child
        gameObject.transform.LookAt(_child.transform);

        gameObject.transform.Rotate(0.0f, 0.0f, _rotation);
        
        //for scaling
        Vector3 ls = gameObject.transform.localScale;

        // get scaling measure
        ls.z = Vector3.Distance(_parent.transform.position, _child.transform.position);
        
        // some additional scaling if the edge is curved
        if (_isCurved) {

            ls.y = Vector3.Distance(_parent.transform.position, _child.transform.position)/3;

            ls.x = Vector3.Distance(_parent.transform.position, _child.transform.position)/8;

        }

        //scale
        gameObject.transform.localScale = ls;

        //move into position
        gameObject.transform.position = new Vector3(
            (_child.transform.position.x+_parent.transform.position.x)/2,
            (_child.transform.position.y+_parent.transform.position.y)/2,
            (_child.transform.position.z+_parent.transform.position.z)/2);
  }
    // constructors are ill advised, so we use this instead
    public void InstantiateEdge(bool isCurvedEdge, Backend.GraphEdge graphEdge, GameObject text, GameObject p, GameObject c, float rotation = 0f) { 

        _isCurved = isCurvedEdge;
        _databaseEdge = graphEdge;
        _textObject = text;
        _parent = p;
        _child = c;
        _rotation = rotation;

        //gameObject.GetComponent<StoreParentChild>().parent = _parent;
        //gameObject.GetComponent<StoreParentChild>().child = _child;
        gameObject.transform.LookAt(_child.transform);
        gameObject.transform.Rotate(0.0f, 0.0f, rotation);

        if (_parent == _child) {

        }
    }
}