using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node: MonoBehaviour
{
    
  GameObject _edgePreFab;

  List<GameObject> edges  = new List<GameObject>();

  List<GameObject> selfReferences  = new List<GameObject>();

  List<SpringJoint> joints = new List<SpringJoint>();  
  
  void Start(){
    //transform.GetChild(0).GetComponent<TextMesh>().text = name;
  }
  
  void Update(){
    foreach (GameObject selfReference in selfReferences) {    
      selfReference.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    int i = 0;
    foreach (GameObject edge in edges){

      edge.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

      SpringJoint sj = joints[i];

      if (sj == null)
        return;

      GameObject target = new GameObject();

      if (sj.connectedBody == null)
        return;

      target = sj.connectedBody.gameObject;

      edge.transform.LookAt(target.transform);

      Vector3 ls = edge.transform.localScale;

      ls.z = Vector3.Distance(transform.position, target.transform.position);
      edge.transform.localScale = ls;

      edge.transform.position = new Vector3(
              (transform.position.x+target.transform.position.x)/2,
					    (transform.position.y+target.transform.position.y)/2,
					    (transform.position.z+target.transform.position.z)/2);
      i++;
    }
  }

  public void SetEdgePrefab(GameObject _edgePreFab){
    this._edgePreFab = _edgePreFab;
  }
  
  public void AddEdge(GameObject n){

    SpringJoint sj = gameObject.AddComponent<SpringJoint> (); 

    sj.autoConfigureConnectedAnchor = false;

    sj.spring = 20;

    sj.anchor = new Vector3(0,0.5f,0);

    sj.connectedAnchor = new Vector3(0,0,0);    

    sj.enableCollision = true;

    //sj.massScale = 0;

    //sj.connectedMassScale = 0;

    //sj.currentForce = 1;

    //sj.currentTorque = 1;

    sj.damper = 5;

    if (n.GetComponent<Rigidbody>() == GetComponent<Rigidbody>()) {
      return;
    }
    
    sj.connectedBody = n.GetComponent<Rigidbody>();

    GameObject edge = Instantiate(this._edgePreFab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

    joints.Add(sj);

    edges.Add(edge);
  }
}


