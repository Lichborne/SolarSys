//Original Code From Tobias J. at https://forum.unity.com/threads/implement-a-drag-and-drop-script-with-c.130515/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
class Drag : MonoBehaviour
{
    public GameObject _edgePrefab;
    public GameObject _selfReferencePreFab;
    private Color mouseOverColor = Color.blue;
    private Color originalColor = Color.grey;
    private bool dragging = false;
    private float distance;

    private float radius = 16;
 
   
    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = mouseOverColor;
    }
 
    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = originalColor;
    }

    void OnMouseOver () {
        if (Input.GetMouseButtonDown(2)){

        Vector3 NewPosition = Vector3.zero;
        
        List<Vector3> hitPoints = new List<Vector3>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (var hitCollider in hitColliders)
        {
            hitPoints.Add(hitCollider.gameObject.transform.position);
        }

        int tries = 0;

        while (tries < 100) {
            NewPosition = new Vector3(gameObject.transform.position.x+Random.Range(-radius/4, radius/4), gameObject.transform.position.y+Random.Range(-radius/4, radius/4), gameObject.transform.position.z+Random.Range(-radius/4, radius/4));

            foreach (Vector3 hitPoint in hitPoints) {
                if (Vector3.Distance(NewPosition, hitPoint) < radius/4) {
                    tries +=1;
                    continue;
                }  
            }
            break;
        }

        if(tries == 100) {
            Debug.Log("No available position found");
        }

        GameObject NewNode = Instantiate(gameObject, NewPosition, Quaternion.identity);

        NewEdge graphEdge = gameObject.AddComponent<NewEdge>();
        graphEdge.InstantiateEdge(_edgePrefab, _selfReferencePreFab, gameObject, NewNode);
            
    }
}
 
    void OnMouseDown()
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }
 
    void OnMouseUp()
    {
        dragging = false;
    }
 
    void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
    }
}
 