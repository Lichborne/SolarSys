using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewEdge : MonoBehaviour
{

    private GameObject _edgePrefab = null;

    private GameObject _selfReferencePrefab = null;

    public GameObject _parent { get; set; } = null;

    public GameObject _child {get; set; } = null;
    // Start is called before the first frame update
    void Start()
    {
        if (_parent == _child) {
            Instantiate(_selfReferencePrefab, _parent.transform.position, _parent.transform.rotation);
            return;
        }

        Instantiate(_edgePrefab, _parent.transform.position, _parent.transform.rotation);

    }

    // Instead of update; moving called by funct in node.
    void Update()
    {
        if (_parent == _child) {
            transform.position = _parent.transform.position;
            transform.rotation = _parent.transform.rotation;
            return;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        transform.LookAt(_parent.transform);

        Vector3 ls = transform.localScale;

        ls.z = Vector3.Distance(transform.position, _parent.transform.position);
        
        transform.localScale = ls;

        transform.position = new Vector3(
              (transform.position.x+_parent.transform.position.x)/2,
					    (transform.position.y+_parent.transform.position.y)/2,
					    (transform.position.z+_parent.transform.position.z)/2);
  }

  public void InstantiateEdge(GameObject epf, GameObject srpf, GameObject p, GameObject c) { 
    _edgePrefab = epf;
    _selfReferencePrefab = srpf;
    _parent = p;
    _child = c;
  }
}
