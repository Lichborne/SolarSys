using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteEdge : MonoBehaviour
{

    void OnMouseOver () {
         if (Input.GetKeyDown("delete")) {
            

            gameObject.GetComponent<FrontEndEdge>()._child.GetComponent<FrontEndNode>().from.Remove(gameObject.GetComponent<FrontEndEdge>()._parent);

            gameObject.GetComponent<FrontEndEdge>()._parent.GetComponent<FrontEndNode>().to.Remove(gameObject.GetComponent<FrontEndEdge>()._child);

            gameObject.GetComponent<FrontEndEdge>()._parent.GetComponent<FrontEndNode>().edgeOut.Remove(gameObject);

            Destroy(gameObject);
        }
    }

}
